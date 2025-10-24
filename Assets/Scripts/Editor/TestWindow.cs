using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public partial class TestWindow : OdinEditorWindow
{
    [MenuItem("Dinotonte/Test Window")]
    private static void OpenWindow()
    {
        GetWindow<TestWindow>().Show();
    }
    public bool Toggle = true;

    [HideIfGroup("Toggle")]
    [BoxGroup("Toggle/Shown Box")]
    public int A, B;

    [BoxGroup("Box")]
    public InfoMessageType EnumField = InfoMessageType.Info;

    [BoxGroup("Box")]
    [HideIfGroup("Box/Toggle")]
    public Vector3 X, Y;

    // Like the regular If-attributes, HideIfGroup also supports specifying values.
    // You can also chain multiple HideIfGroup attributes together for more complex behaviour.
    [HideIfGroup("Box/Toggle/EnumField", Value = InfoMessageType.Info)]
    [BoxGroup("Box/Toggle/EnumField/Border", ShowLabel = false)]
    public string Name;

    [BoxGroup("Box/Toggle/EnumField/Border")]
    public Vector3 Vector;

    // HideIfGroup will by default use the name of the group,
    // but you can also use the MemberName property to override this.
    [HideIfGroup("RectGroup", Condition = "Toggle")]
    public Rect Rect;

    [TitleGroup("Ints")]
    public int SomeInt1;

    [TitleGroup("$SomeString1", "Optional subtitle")]
    public string SomeString1;

    [TitleGroup("$striiing")]
    public string striiing;

    [TitleGroup("Vectors", "Optional subtitle", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: false)]
    public Vector2 SomeVector1;

    [TitleGroup("Ints", "Optional subtitle", alignment: TitleAlignments.Split)]
    public int SomeInt2;

    [TitleGroup("$SomeString1", "Optional subtitle")]
    public string SomeString2;

    [TitleGroup("Vectors")]
    public Vector2 SomeVector2 { get; set; }

    [TitleGroup("Ints/Buttons", indent: false)]
    private void IntButton() { }

    [TitleGroup("$SomeString1/Buttons", indent: false)]
    private void StringButton() { }

    [TitleGroup("Vectors")]
    private void VectorButton() { }
}