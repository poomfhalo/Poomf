using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Contains information about the audio settings that allow audio sources to sync
// their audio settings without being able to manipulate the global settings
public interface IAudioProvider
{
    float GetSFXVolume();
    float GetMusicVolume();
}