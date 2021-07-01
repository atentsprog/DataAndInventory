using UnityEngine;

public class SaveString
{
    readonly string key;
    string value;
    public SaveString(string key)
    {
        this.key = Application.dataPath + key;
    }

    public string Value
    {
        get
        {
            if (value == null)
            {
                if (PlayerPrefs.HasKey(key))
                    value = PlayerPrefs.GetString(key);
                else
                    value = "";
            }
            return value;
        }
        set
        {
            if (this.value != value)
            {
                PlayerPrefs.SetString(key, value);
                PlayerPrefs.Save();
            }
            this.value = value;
        }
    }

    public override string ToString()
    {
        return Value;
    }
}