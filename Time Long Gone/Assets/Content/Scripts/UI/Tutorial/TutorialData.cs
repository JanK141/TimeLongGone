using System;
using System.Collections.Generic;

[Serializable]
public class TutorialData
{
    public string title;
    public string flavour;
    public string instruction;
}

[Serializable]
public class JsonTutorialList
{
    // naming of field must match in JSON
    public List<TutorialData> tutorialsData;
}