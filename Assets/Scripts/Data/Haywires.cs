using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class HaywireCollection
{
    private List<Haywire> haywires;

    public HaywireCollection(int haywireCount) {
        haywires = new List<Haywire>();

        // Generate
        List<Haywire> remainingHaywires = new List<Haywire>();
        IEnumerable<HaywireType> types = Enum.GetValues(typeof(HaywireType)).Cast<HaywireType>();
        foreach (HaywireType type in types) {
            remainingHaywires.Add(
                new Haywire {
                    Type = type,
                    Active = false,
                    Chance = type == HaywireType.ShipVisibilityReduced ? 1 : 2
                }
            );
        }

        // Set active
        while (haywires.Count < haywireCount) {
            int haywireNumber = UnityEngine.Random.Range(0, remainingHaywires.Select(x => x.Chance).Count());

            Haywire selectedHaywire = null;
            int chanceTracker = 0;
            foreach (Haywire h in remainingHaywires) {
                selectedHaywire = h;
                chanceTracker += h.Chance;
                if (chanceTracker > haywireNumber) {
                    break;
                }
            }

            selectedHaywire.Active = true;
            haywires.Add(selectedHaywire);
            remainingHaywires.Add(selectedHaywire);
        }

        foreach (Haywire h in haywires) {
            Debug.Log(h.Type);
        }

        Debug.Log(TotalHaywires);
    }

    public bool IsActive(HaywireType type) {
        Haywire haywire = haywires.Find(x => x.Type == type);
        if (haywire == null) {
            return false;
        }

        return haywire.Active;
    }

    public void SetActive(HaywireType type, bool active = true) {
        Haywire haywire = haywires.Find(x => x.Type == type);
        if (haywire == null) {
            return;
        }

        haywire.Active = active;
    }

    public int TotalHaywires {
        get {
            return haywires.Where(x => x.Active).Count();
        }
    }
}

public class Haywire {
    public HaywireType Type;
    public bool Active;
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