using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Toggle textToggle;
    [SerializeField] private Toggle hotelToggle;
    [SerializeField] private Toggle whiteBlackListsToggle;

    [SerializeField] private Animator settingsAnimator;
    public float Duration => settingsAnimator.GetCurrentAnimatorStateInfo(0).length;
    public static bool IsShown { get; private set; }

    private const string SearchNumberKey = "SearchOnSettingNumber";
    private const string SearchHotelKey = "SearchOnSettingHotelUsing";
    private const string SearchListsKey = "SearchOnChangingWhiteBlackLists";

    public static bool SearchOnSettingNumber { get; private set; }
    public static bool SearchOnSettingHotelUsing { get; private set; }
    public static bool SearchOnChangingWhiteBlackLists { get; private set; }

    private void Start() => LoadSettings();

    public void ChangeSettingsWindowState(bool toOpen)
    {
        IsShown = toOpen;
        settingsAnimator.SetBool("opened", toOpen);
    }

    private void LoadSettings()
    {
        SearchOnSettingNumber = PlayerPrefs.GetInt(SearchNumberKey, 0) == 1;
        SearchOnSettingHotelUsing = PlayerPrefs.GetInt(SearchHotelKey, 0) == 1;
        SearchOnChangingWhiteBlackLists = PlayerPrefs.GetInt(SearchListsKey, 1) == 1;
        textToggle.isOn = SearchOnSettingNumber;
        hotelToggle.isOn = SearchOnSettingHotelUsing;
        whiteBlackListsToggle.isOn = SearchOnChangingWhiteBlackLists;
    }

    public void SaveSearchOnSettingNumber(bool value) 
        => SaveSetting(value, SearchNumberKey, v => SearchOnSettingNumber = v);

    public void SaveSearchOnSettingHotelUsing(bool value) 
        => SaveSetting(value, SearchHotelKey, v => SearchOnSettingHotelUsing = v);

    public void SaveSearchOnChangingWhiteBlackLists(bool value) 
        => SaveSetting(value, SearchListsKey, v => SearchOnChangingWhiteBlackLists = v);

    private void SaveSetting(bool value, string key, Action<bool> setter)
    {
        setter(value);
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
