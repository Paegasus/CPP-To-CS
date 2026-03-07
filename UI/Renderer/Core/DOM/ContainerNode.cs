using UI.Renderer.Core.DOM.Events;

namespace UI.Renderer.Core.DOM;

// ContainerNode itself isn't web-exposed exactly, but it maps closely to the
// ParentNode mixin interface. A number of methods it implements (such as
// firstChild, lastChild) use web-style naming to shadow the corresponding
// methods on Node. This is a performance optimization, as it avoids a virtual
// dispatch if the type is statically known to be ContainerNode.
public class ContainerNode : Node
{
    
}
