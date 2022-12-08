using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;



public enum Terrain
{
    land,
    sea,
    air
}

public class GlobalVariables
{
    public static Dictionary<string, Weapon> weaponList;
    public static Dictionary<string, UClass> unitClassesList;
    public static Dictionary<string, Unit> playerUnitsList;
    public static Dictionary<string, Unit> enemyUnitsList;
}
public class Program : GlobalVariables
{
    public static void Main(string[] args)
    {
        LoadWeapons();
        LoadClasses();
        LoadPlayerUnits();

        Console.WriteLine(playerUnitsList["Bravo Team"].name + playerUnitsList["Bravo Team"].isAlive);
    }
    public static void LoadWeapons()
    {
        weaponList = JsonConvert.DeserializeObject<Dictionary<string, Weapon>>(ReadJSON("Weapons"));
    }
    public static void LoadClasses()
    {
        unitClassesList = JsonConvert.DeserializeObject<Dictionary<string, UClass>>(ReadJSON("Classes"));
    }
    public static void LoadPlayerUnits()
    {
        playerUnitsList = JsonConvert.DeserializeObject<Dictionary<string, Unit>>(ReadJSON("PlayerUnits"));
    }
    public static string ReadJSON(string file)
    {
        using (StreamReader r = new StreamReader(@$"JSONs\{file}.json"))
        {
            string json = r.ReadToEnd();
            return json;
        }
    }
}
public class Unit : GlobalVariables
{
    public string name { get; set; }
    public UClass unitClass { get; set; }
    public Weapon[]? weapons { get; set; }
    public int availableSlots { get; set; }
    public bool isAlive { get; set; }

    public Unit(string uName, UClass uClass, Weapon[]? uWeapons = null)
    {
        name = uName;
        unitClass = uClass;
        weapons = uWeapons;
        availableSlots = uClass.weaponSlots;
        isAlive = true;
    }

    [JsonConstructor]
    public Unit(string uName, string uClass, string[]? uWeapons, bool uIsAlive)
    {
        name = uName;
        unitClass = unitClassesList[uClass];
        availableSlots = unitClass.weaponSlots;

        int i = 0;
        foreach (string? name in uWeapons)
        {
            weapons[i] = weaponList[name];
            availableSlots -= weapons[i].slotSpace;
            i++;
        }

        isAlive = uIsAlive;
    }

    public void setWeapon(string newWeapon)
    {
        if (weapons == null)
        {
            weapons = new Weapon[unitClass.weaponSlots];
        }

        if (availableSlots >= GlobalVariables.weaponList[newWeapon].slotSpace)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] == null)
                {
                    weapons[i] = GlobalVariables.weaponList[newWeapon];
                    availableSlots -= weapons[i].slotSpace;
                    break;
                }
            }
        }
    }
}
public class Weapon
{
    public string name;
    public Terrain[] validTargets;
    public float damage;
    public float accuracity;
    public int shotsPerFire;
    public int slotSpace;

    public Weapon(string wName, Terrain[] wValidTargets, float wDamage, float wAccuracity, int wShotsPerFire, int wSlotSpace)
    {
        name = wName;
        validTargets = wValidTargets;
        damage = wDamage;
        accuracity = wAccuracity;
        shotsPerFire = wShotsPerFire;
        slotSpace = wSlotSpace;
    }
}
public class UClass
{
    public string name;
    public Terrain terrain;
    public float hp;
    public float armor;
    public int weaponSlots;

    public UClass(string cName, Terrain cTerrain, float cHP, float cArmor, int cWeaponSlots)
    {
        name = cName;
        terrain = cTerrain;
        hp = cHP;
        armor = cArmor;
        weaponSlots = cWeaponSlots;
    }
}