using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IItem { 

    string   Name        { get;}
    int      MaxCount    { get;}
    string   Descript    { get;}
    Image    Shape       { get;}
    bool     Usable      { get;}
    bool     Wasteable   { get;}
}

public class HealItem: IItem {

    public string   Name        { get; private set; }
    public int      MaxCount    { get; private set; }
    public string   Descript    { get; private set; }
    public Image    Shape       { get; private set; }
    public bool     Usable      { get; private set; }
    public bool     Wasteable   { get; private set; }

    private bool    scalarType;
    public  bool    AllPeople   { get; private set; }
    public  float   EffectPower { get; private set; }
}

public class AttackBuffItem : IItem {

    public  string  Name { get; private set; }
    public  int     MaxCount { get; private set; }
    public  string  Descript { get; private set; }
    public  Image   Shape { get; private set; }
    public  bool    Usable { get; private set; }
    public  bool    Wasteable { get; private set; }

    private bool    scalarType;
    public  bool    AllPeople   { get; private set; }
    public  float   EffectPower { get; private set; }
    public  int     Duraction   { get; private set; }
}

public class ShieldBuffItem : IItem {

    public string   Name        { get; private set; }
    public int      MaxCount    { get; private set; }
    public string   Descript    { get; private set; }
    public Image    Shape       { get; private set; }
    public bool     Usable      { get; private set; }
    public bool     Wasteable   { get; private set; }

    private bool scalarType;
    public bool AllPeople       { get; private set; }
    public float EffectPower    { get; private set; }
    public int Duraction        { get; private set; }
}

public class ImportantItem : IItem {

    public string   Name        { get; private set; }
    public int      MaxCount    { get; private set; }
    public string   Descript    { get; private set; }
    public Image    Shape       { get; private set; }
    public bool     Usable      { get; private set; } = false;
    public bool     Wasteable   { get; private set; } = false;
}