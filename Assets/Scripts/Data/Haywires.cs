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
        List<HaywireDefinition> remainingHaywires = HaywireHelper.GetAllHaywires();

        // Set active
        Debug.Log(remainingHaywires.Sum(x => x.Chance));
        while (haywires.Count < haywireCount && remainingHaywires.Count > 0) {
            int haywireNumber = UnityEngine.Random.Range(0, remainingHaywires.Sum(x => x.Chance));

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

public static class HaywireHelper {
    public static List<HaywireDefinition> GetAllHaywires() {
        return new List<HaywireDefinition> {
            new HaywireDefinition(HaywireType.ShipMovementVerticalOnly),
            new HaywireDefinition(HaywireType.ShipSpeedDoubled),
            new HaywireDefinition(HaywireType.ShipCannonsSpin),
            new HaywireDefinition(HaywireType.ShipSpinUncontrollable),
            new HaywireDefinition(HaywireType.ShipArmorWeightIncreased),
            new HaywireDefinition(HaywireType.ShipMovementVerticalUncontrollable)
        };
    }
}

public class HaywireDefinition {
    public HaywireDefinition(HaywireType type) {
        Type = type;
    }

    public HaywireType Type;
    public int Chance = 2;
}

public enum HaywireType {
    ShipMovementVerticalOnly = 0,       // DONE
    ShipMovementVerticalUncontrollable = 1,
    ShipSpeedDoubled = 2,               // DONE
    ShipCannonsNonStop = 3,
    ShipCannonsSpin = 4,                // DONE
    ShipProjectilesWeighted = 5,   
    ShipCannonsBackwards = 6,
    ShipVisibilityReduced = 7,
    ShipSpinUncontrollable = 8,         // DONE
    ShipArmorWeightIncreased = 9        // DONE
}