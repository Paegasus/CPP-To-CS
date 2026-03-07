using UI.Renderer.Core.DOM;
using UI.Renderer.Framework.Text;

namespace UI.Renderer.Core.CSS;

public class StyleElement
{
    public enum ProcessingResult
    {
        kProcessingSuccessful,
        kProcessingFatalError
    }
    
    // We want CSS Modules to behave similar to the "already started" flag Import
    // Maps, essentially making it a one-shot operation when the <style> element
    // is first connected. This behavior is subject to change based on WHATWG
    // feedback. Once set on a given element, these types cannot change.
    // TODO(crbug.com/448174611): Update this behavior based on WHATWG feedback.
    private enum StyleType
    {
        kPending,  // Still unknown.
        kClassic,  // Definitely a classic style tag.
        kModule    // Definitely a declarative CSS module.
    }

    private bool has_finished_parsing_children_ = true;
    private bool loading_ = true;
    private bool registered_as_candidate_ = true;
    private bool created_by_parser_ = true;
    private StyleType element_type_ = StyleType.kPending;
    private TextPosition start_position_;
    private PendingSheetType pending_sheet_type_;
    private RenderBlockingBehavior render_blocking_behavior_;

    private ProcessingResult CreateSheetOrModule(Element, string text)
    {
        throw new NotImplementedException();
    }

    private void AddImportMapEntry(Element, string text) { throw new NotImplementedException(); }

    private ProcessingResult Process(Element) { throw new NotImplementedException(); }

    private void ClearSheet(Element owner_element) { throw new NotImplementedException(); }

    public bool CreatedByParser { get => created_by_parser_; }

    public CSSStyleSheet? Sheet => sheet_?.Get();
    public bool IsModule { get; }
    
    virtual  AtomicString type() const = 0;
    virtual  AtomicString media() const = 0;

    // Returns whether |this| and |node| are the same object. Helps us verify
    // parameter validity in certain member functions with an Element parameter
    // which should only be called by a subclass with |this|.
    virtual bool IsSameObject(const Node& node) const = 0;
    public CSSStyleSheet sheet() { return sheet_.Get(); }
    public bool IsLoading() { throw new NotFiniteNumberException(); }
    public bool SheetLoaded(Document document){ throw new NotFiniteNumberException(); }
    public void SetToPendingState(Document document, Element element) { throw new NotFiniteNumberException(); }
    public void RemovedFrom(Element element, ContainerNode insertionPoint) { throw new NotFiniteNumberException(); }
    public void BlockingAttributeChanged(Element element) { throw new NotFiniteNumberException(); }
    public ProcessingResult ProcessStyleSheet(Document document, Element element) { throw new NotFiniteNumberException(); }
    public ProcessingResult ChildrenChanged(Element element) { throw new NotFiniteNumberException(); }
    public ProcessingResult FinishParsingChildren(Element element) { throw new NotFiniteNumberException(); }
}