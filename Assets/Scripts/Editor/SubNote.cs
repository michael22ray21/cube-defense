using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum ContentType
{
    Space,
    Text,
    Title,
    Image
}
[Serializable]
public class SubNote
{
    #region Vars, Fields, Getters
    [HorizontalGroup("Row", Width = 100)]
    [HideLabel]
    [ValueDropdown("GetContentTypes")]
    public ContentType type = ContentType.Text;

    [HorizontalGroup("Row")]
    [HideLabel]
    [ShowIf("@type == ContentType.Text || type == ContentType.Title")]
    [MultiLineProperty(3)]
    public string textValue = "";

    [HorizontalGroup("Row")]
    [HideLabel]
    [ShowIf("@type == ContentType.Image")]
    [PreviewField(50, ObjectFieldAlignment.Left)]
    public Texture2D imageValue;

    private static IEnumerable<ValueDropdownItem<ContentType>> GetContentTypes()
    {
        return new ValueDropdownList<ContentType>()
        {
            { "Space", ContentType.Space },
            { "Text", ContentType.Text },
            { "Title", ContentType.Title },
            { "Image", ContentType.Image }
        };
    }
    #endregion

    #region Behavior
    public string GetLabel()
    {
        return type switch
        {
            ContentType.Space => "Space",
            ContentType.Text => string.IsNullOrEmpty(textValue) ? "Text (empty)" :
                                    textValue.Length > 30 ? textValue[..30] + "..." : textValue,
            ContentType.Title => string.IsNullOrEmpty(textValue) ? "Title (empty)" :
                                    "Title: " + (textValue.Length > 25 ? textValue[..25] + "..." : textValue),
            ContentType.Image => imageValue == null ? "Image (none)" : "Image: " + imageValue.name,
            _ => type.ToString(),
        };
    }
    #endregion
}