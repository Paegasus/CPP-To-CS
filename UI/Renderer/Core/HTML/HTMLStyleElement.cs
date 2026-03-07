using UI.Renderer.Core.DOM;
using UI.Renderer.Core.CSS;

namespace UI.Renderer.Core.HTML;

public sealed class HTMLStyleElement : HTMLElement
{
    // Private inheritance → private field (composition)
    private readonly StyleElement style_element_;

    private BlockingAttribute blocking_attribute_;

    public HTMLStyleElement(Document document, CreateElementFlags flags = default) : base(document, flags)
    {
        style_element_ = new StyleElement(document, createdByParser: false);
    }

    // "using StyleElement::sheet" — expose sheet publicly via delegation
    public CSSStyleSheet? Sheet => style_element_.Sheet;

    // "using StyleElement::IsModule" — expose publicly via delegation
    public bool IsModule => style_element_.IsModule;

    public bool Disabled { get; set; }

    public BlockingAttribute Blocking => blocking_attribute_.Value;

    // StyleElement's abstract members are implemented here and delegated
    protected override string Media => GetAttribute(html_names.kMediaAttr) ?? string.Empty;

    protected override string Type => GetAttribute(html_names.kTypeAttr) ?? string.Empty;

    // HTMLElement overrides
    protected override void ParseAttribute(AttributeModificationParams parameters)
    {
        // ...
        style_element_.BlockingAttributeChanged(this);
    }

    protected override InsertionNotificationRequest InsertedInto(ContainerNode insertionPoint)
    {
        var result = base.InsertedInto(insertionPoint);
        style_element_.ProcessStyleSheet(GetDocument(), this);
        return result;
    }

    protected override void RemovedFrom(ContainerNode insertionPoint)
    {
        base.RemovedFrom(insertionPoint);
        style_element_.RemovedFrom(this, insertionPoint);
    }

    protected override void ChildrenChanged(ChildrenChange change)
    {
        base.ChildrenChanged(change);
        style_element_.ChildrenChanged(this);
    }

    protected override void FinishParsingChildren()
    {
        base.FinishParsingChildren();
        style_element_.FinishParsingChildren(this);
    }

    public override bool SheetLoaded() => style_element_.SheetLoaded(GetDocument());

    public override void SetToPendingState() => style_element_.SetToPendingState(GetDocument(), this);
}
