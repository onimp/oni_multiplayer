#!/bin/bash
# Automated entity size regression tests for ONI DedicatedServer
# Usage: ./test_entity_sizes.sh [base_url]
# Server must be running. Default URL: http://localhost:8080

BASE_URL="${1:-http://localhost:8080}"
FAILED=0
PASSED=0
SKIPPED=0

# Fetch entities once and reuse
RESPONSE=$(curl -s --max-time 10 "$BASE_URL/api/entities" 2>/dev/null)
if [ -z "$RESPONSE" ]; then
    echo "ERROR: Could not reach $BASE_URL/api/entities — is the server running?"
    exit 1
fi
ENTITY_COUNT=$(echo "$RESPONSE" | python3 -c "import sys,json; d=json.load(sys.stdin); print(len(d.get('entities',[])))" 2>/dev/null)
TICK=$(echo "$RESPONSE" | python3 -c "import sys,json; d=json.load(sys.stdin); print(d.get('tick','?'))" 2>/dev/null)
echo "=== Entity Size Regression Tests ==="
echo "Server: $BASE_URL | tick=$TICK | entities=$ENTITY_COUNT"
echo ""

# assert_size <entity_name> <expected_w> <expected_h> [entity_type_filter]
# Matches first entity where name == entity_name (optionally filtered by type).
# Prints PASS/FAIL/SKIP and updates counters.
assert_size() {
    local label="$1"
    local expected_w="$2"
    local expected_h="$3"
    local type_filter="${4:-}"  # optional: "building", "critter", "duplicant", etc.

    local result
    result=$(echo "$RESPONSE" | python3 -c "
import sys, json
data = json.load(sys.stdin)
entities = data.get('entities', [])
name_filter = '$label'
type_f = '$type_filter'
matches = [e for e in entities if e.get('name','') == name_filter and (not type_f or e.get('type','') == type_f)]
if not matches:
    print('NOT_FOUND')
else:
    e = matches[0]
    print(str(e.get('w','?')) + ' ' + str(e.get('h','?')))
" 2>/dev/null)

    if [ "$result" = "NOT_FOUND" ]; then
        echo "SKIP: $label — not found in world (entity may not be spawned)"
        SKIPPED=$((SKIPPED+1))
        return
    fi

    local actual_w actual_h
    actual_w=$(echo "$result" | cut -d' ' -f1)
    actual_h=$(echo "$result" | cut -d' ' -f2)

    if [ "$actual_w" = "$expected_w" ] && [ "$actual_h" = "$expected_h" ]; then
        echo "PASS: $label = ${actual_w}x${actual_h}"
        PASSED=$((PASSED+1))
    else
        echo "FAIL: $label — expected ${expected_w}x${expected_h}, got ${actual_w}x${actual_h}"
        FAILED=$((FAILED+1))
    fi
}

# ── Building sizes ─────────────────────────────────────────────────────────────
# Headquarters (Command Module): HeadquartersConfig.CreateBuildingDef("Headquarters", 4, 4, ...)
assert_size "Headquarters" 4 4 "building"

# Telepad (Printing Pod): size from BuildingDef (skipped if not present in world)
assert_size "Telepad" 4 3 "building"

# ── Duplicant sizes ────────────────────────────────────────────────────────────
# Minion: KBoxCollider2D.size=(1, 1.5) → rounds to 1×2
assert_size "Minion" 1 2 "duplicant"

# ── Critter sizes ──────────────────────────────────────────────────────────────
# Hatch: BaseHatchConfig.CreatePlacedEntity(..., width:1, height:1) → 1×1
assert_size "Hatch" 1 1 "critter"

# Puft: BasePuftConfig.CreatePlacedEntity(..., width:1, height:1) → 1×1
assert_size "Puft" 1 1 "critter"

# ── Default / unknown entity fallback ─────────────────────────────────────────
# Entities without explicit size rules should default to 1×1
# Use a simple pickupable item as a proxy for the default case
assert_size "SlimeMold" 1 1 "ore"

echo ""
echo "=== Results: $PASSED passed, $FAILED failed, $SKIPPED skipped ==="
if [ "$FAILED" -gt 0 ]; then
    echo "REGRESSION DETECTED — $FAILED test(s) failed"
    exit 1
fi
exit 0
