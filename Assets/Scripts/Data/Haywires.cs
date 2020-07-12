using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class HaywireCollection
{
    private List<HaywireType> haywires;

    public HaywireCollection(int haywireCount) {
        haywires = new List<HaywireType>();

        // Generate
        List<HaywireDefinition> remainingHaywires = new List<HaywireDefinition>();
        IEnumerable<HaywireType> types = Enum.GetValues(typeof(HaywireType)).Cast<HaywireType>();
        foreach (HaywireType type in types) {
            remainingHaywires.Add(
                new HaywireDefinition {
                    Type = type,
                    Chance = type == HaywireType.ShipVisibilityReduced ? 1 : 2
                }
            );
        }

        // Set active
        while (haywires.Count < haywireCount) {
            int haywireNumber = UnityEngine.Random.Range(0, remainingHaywires.Select(x => x.Chance).Count());

            HaywireDefinition selectedHaywire = null;
            int chanceTracker = 0;
            foreach (HaywireDefinition h in remainingHaywires) {
                selectedHaywire = h;
                chanceTracker += h.Chance;
                if (chanceTracker > haywireNumber) {
                    break;
                }
            }

            haywires.Add(selectedHaywire.Type);
            remainingHaywires.Add(selectedHaywire);
        }
    }

    public HaywireCollection(List<HaywireType> types) {
        haywires = types;
    }

    public bool IsActive(HaywireType type) {
        return haywires.Contains(type);
    }

    public void SetActive(HaywireType type, bool active = true) {
        // If active is false, remove and return
        if (!active) {
            haywires.Remove(type);
            return;
        }

        // Else, add if not present
        if (!haywires.Contains(type)) {
            haywires.Add(type);
        }
    }

    public int TotalHaywires => haywires.Count;
}

public class HaywireDefinition {
    public HaywireType Type;
    public int Chance;
}

public enum HaywireType {
    ShipMovementVerticalOnly = 0,
    ShipMovementPong = 1,
    ShipSpeedDoubled = 2,
    ShipCannonsNonStop = 3,
    ShipCannonsSpin = 4,
    ShipProjectilesWeighted = 5,
    ShipProjectilesPersistent = 6,
    ShipVisibilityReduced = 7,
    ShipSpinUncontrollable = 8
}