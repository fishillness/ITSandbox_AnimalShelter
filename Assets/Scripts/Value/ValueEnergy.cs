using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ValueEnergy : MonoBehaviour
{
    public const string Filename = "EnergyTime313131";

    [SerializeField] private int energy_Recovering;
    [SerializeField] private float time_Restoration;
    [SerializeField] private ValueManager valueManager;

    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    private float startTime;
    private bool stopTimer;
    

    [SerializeField]
    public class EnergyTimeData
    {
        public DateTime time;
    }

    private IEnumerator Start()
    {
        stopTimer = false;
        startTime = time_Restoration * 60;
        if ((DateTime.Now - LoadStoreData()).TotalMinutes >= time_Restoration)
        {
            var oldTime = LoadStoreData();
            var dif = (DateTime.Now - oldTime).TotalMinutes;

            var n = dif % energy_Recovering * 60;
            DateTimeToUnixTimestamp();
            if (valueManager.SetCurrent() == valueManager.SetMax())
            {
                valueManager.UpdateTime("0:00");
                stopTimer = true;
            }
            else
            {
                valueManager.UpdateTime(n.ToString());
            }
        }
        else
        {
            if (valueManager.SetCurrent() == valueManager.SetMax())
            {
                valueManager.UpdateTime("0:00");
                stopTimer = true;
            }
            else
            {
                yield return new WaitForSeconds((float)(DateTime.Now - LoadStoreData()).TotalSeconds);
                AddEnergy();
            }
        }

        valueManager.DeleteResources(0, 0, 0, 0, 30);
    }

    private void Update()
    {
        StartCoroutine(TextOn());
    }

    private void DateTimeToUnixTimestamp()
    {
        var oldTime = LoadStoreData();
        var dif = (DateTime.Now - oldTime).TotalMinutes;

            var n = (int)dif / energy_Recovering;
            valueManager.AddResources(0, 0, 0, 0, energy: energy_Recovering * n);
            SaveStoreData();
    }

    private void SaveStoreData()
    {
        EnergyTimeData storeData = new EnergyTimeData();

        storeData.time = DateTime.Now;

        Saver<EnergyTimeData>.Save(Filename, storeData);
    }

    private DateTime LoadStoreData()
    {

        EnergyTimeData storeData = new EnergyTimeData();

        if (Saver<EnergyTimeData>.TryLoad(Filename, ref storeData) == false)
        {
            return DateTime.Now;
        }
        else
        {
            return storeData.time;
        }
    }

    private void AddEnergy()
    {
        valueManager.AddResources(0, 0, 0, 0, energy: energy_Recovering);
        SaveStoreData();
        startTime = time_Restoration * 60;
    }

    private IEnumerator TextOn()
    {
        if (valueManager.SetCurrent() != valueManager.SetMax())
        {
            float time = startTime - Time.time;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time - minutes * 60);

            string textTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            if (stopTimer == false)
            {
                valueManager.UpdateTime(textTime);
            }

            if (time <= 0)
            {
                AddEnergy();
                yield return new WaitForSeconds(1);
                
            }
        }
        else
        {
            valueManager.UpdateTime("0:00");
            stopTimer = true;
        }
    }
}