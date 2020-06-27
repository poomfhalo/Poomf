using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUICommunicationProvider
{
    void ShowScreen(string screenID);
    void HideScreen(string screenID);
}
