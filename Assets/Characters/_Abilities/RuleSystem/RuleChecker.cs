using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleChecker : MonoBehaviour
{
    public Rule rule;
    
    private List<string> errorList;
    public List<string> ErrorList => errorList;

    public void checkRule()
    {
        errorList.Clear();
        errorList.Add("Everything looks good");
    }
}
