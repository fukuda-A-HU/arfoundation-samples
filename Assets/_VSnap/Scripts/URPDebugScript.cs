using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class URPDebugScript : MonoBehaviour
{
    [SerializeField] Material mat;

    [SerializeField] int index;

    [SerializeField] float value;

    [SerializeField] Color color;

    [SerializeField] string propertyName;

    private ShaderPropertyType type;

    void Start()
    {
        GetShaderProperties();
    }

    private void GetShaderProperties()
    {
        //Shaderのプロパティの数を取得し、名前とIDを登録する
        for (int i = 0; i < mat.shader.GetPropertyCount(); i++)
        {
            var _name = mat.shader.GetPropertyName(i);
            int id = mat.shader.GetPropertyNameId(i);
            var _type = mat.shader.GetPropertyType(i);
            Debug.Log("mat " + i + " " + _name + " " + id +  " " + _type.ToString());
        }
    }

    private void Update(){

        propertyName = mat.shader.GetPropertyName(index);

        if(mat.shader.GetPropertyType(index) == ShaderPropertyType.Color){
            mat.SetColor(propertyName, color);
        }
        else if(mat.shader.GetPropertyType(index) == ShaderPropertyType.Float){
            mat.SetFloat(propertyName, value);
        }
    }
}
