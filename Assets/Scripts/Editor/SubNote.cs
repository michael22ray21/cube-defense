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
    [SerializeField]
    [HorizontalGroup("Row", Width = 100)]
    [HideLabel]
    [ValueDropdown("GetContentTypes")]
    private ContentType _type = ContentType.Text;
    public ContentType Type => _type;

    [SerializeField]
    [HorizontalGroup("Row")]
    [HideLabel]
    [ShowIf("@_type == ContentType.Text || _type == ContentType.Title")]
    [MultiLineProperty(3)]
    private string _textValue = "";
    public string TextValue => _textValue;

    [SerializeField]
    [HorizontalGroup("Row")]
    [HideLabel]
    [ShowIf("@_type == ContentType.Image")]
    [PreviewField(50, ObjectFieldAlignment.Left)]
    private Texture2D _imageValue;
    public Texture2D ImageValue => _imageValue;

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
        return _type switch
        {
            ContentType.Space => "Space",
            ContentType.Text => string.IsNullOrEmpty(_textValue) ? "Text (empty)" :
                                    _textValue.Length > 30 ? _textValue[..30] + "..." : _textValue,
            ContentType.Title => string.IsNullOrEmpty(_textValue) ? "Title (empty)" :
                                    "Title: " + (_textValue.Length > 25 ? _textValue[..25] + "..." : _textValue),
            ContentType.Image => _imageValue == null ? "Image (none)" : "Image: " + _imageValue.name,
            _ => _type.ToString(),
        };
    }
    #endregion
}