﻿using UnityEngine;
using TMPro;

public class Rink : MonoBehaviour
{
    [SerializeField] private Puck _puck;
    [SerializeField] private Team _leftTeam;
    [SerializeField] private Team _rightTeam;
    [SerializeField] private Goal _leftGoal;
    [SerializeField] private Goal _rightGoal;
    [SerializeField] private int _pitchRegionsRowsNumber;
    [SerializeField] private int _pitchRegionsColumnsNumber;
    [SerializeField] private GameObject _pitchRegionIndexPref;
    [SerializeField] private Transform _pitchRegionsIndexesHolder;

    private Region _playground;
    private Region[] _pitchRegions;
    private GameObject[] _regionsTexts;
    private int _showHideRegionsCounter;

    public void InitRink()
    {
        SetRinkRegoins();
    }

    private void SetRinkRegoins()
    {
        _playground = new Region(GetComponent<SpriteRenderer>());
        float oneRegionHeight = _playground.GetHeight() / _pitchRegionsRowsNumber;
        float oneRegionWidth = _playground.GetWidth() / _pitchRegionsColumnsNumber;
        _pitchRegions = new Region[_pitchRegionsRowsNumber * _pitchRegionsColumnsNumber];
        _regionsTexts = new GameObject[_pitchRegionsRowsNumber * _pitchRegionsColumnsNumber];
        int regionIndex;
        int k = 0;
        for (int i = 0; i < _pitchRegionsRowsNumber; i++)
        {
            for (int j = 0; j < _pitchRegionsColumnsNumber; j++)
            {
                regionIndex = i * _pitchRegionsColumnsNumber + j;
                _pitchRegions[regionIndex] = new Region(new Vector2(_playground.TopLeft.x + oneRegionWidth * j,
                    _playground.TopLeft.y - oneRegionHeight * i),
                    oneRegionWidth,
                    oneRegionHeight);
                GameObject regionText = Instantiate(_pitchRegionIndexPref, _pitchRegionsIndexesHolder);
                regionText.transform.position = _pitchRegions[regionIndex].Center;
                regionText.GetComponent<TextMeshPro>().text = regionIndex.ToString();
                _regionsTexts[k++] = regionText;
                regionText.SetActive(false);
            }
        }
    }

    public void ShowRegions()
    {
        _showHideRegionsCounter++;
        for (int i = 0; i < _regionsTexts.Length; i++)
        {
            _regionsTexts[i].SetActive(true);
        }
    }

    public void HideRegions()
    {
        _showHideRegionsCounter--;
        if (_showHideRegionsCounter == 0)
        {
            for (int i = 0; i < _regionsTexts.Length; i++)
            {
                _regionsTexts[i].SetActive(false);
            }
        }
    }

    public Region[] GetRinkRegions()
    {
        return _pitchRegions;
    }

    public Puck GetPuck()
    {
        return _puck;
    }

    public bool IsGameOn()
    {
        if (_leftTeam.GetCurrentState() != KickOff.Instance() &&
            _rightTeam.GetCurrentState() != KickOff.Instance())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GoalKeeperHasPuck()
    {
        return false;
    }

    public float GetRinkLength()
    {
        return _playground.GetWidth();
    }
}