using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AirBattle.UI.Menu
{
    public class MenuUILoginField : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            string loginName = PlayerPrefs.GetString(MenuMultiplayerUI.LoginNamePref);
            if (loginName.Length > 0)
            {
                GetComponent<TMP_InputField>().text = loginName;
            }
        }
    }
}
