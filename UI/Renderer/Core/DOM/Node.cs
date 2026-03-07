using UI.Renderer.Core.DOM.Events;

namespace UI.Renderer.Core.DOM;

// A Node is a base class for all objects in the DOM tree.
// The spec governing this interface can be found here:
// https://dom.spec.whatwg.org/#interface-node
public class Node : EventTarget
{
    public enum NodeType
    {
        kElementNode = 1,
        kAttributeNode = 2,
        kTextNode = 3,
        kCdataSectionNode = 4,
        kProcessingInstructionNode = 7,
        kCommentNode = 8,
        kDocumentNode = 9,
        kDocumentTypeNode = 10,
        kDocumentFragmentNode = 11,
    }

    public enum DocumentPosition
    {
        kDocumentPositionEquivalent = 0x00,
        kDocumentPositionDisconnected = 0x01,
        kDocumentPositionPreceding = 0x02,
        kDocumentPositionFollowing = 0x04,
        kDocumentPositionContains = 0x08,
        kDocumentPositionContainedBy = 0x10,
        kDocumentPositionImplementationSpecific = 0x20,
    }

    public static Node FromDomNodeId(DOMNodeId dom_node_id)
    {
        throw new NotImplementedException();
    }

    // Returns the existing DOMNodeID for the node if it has already been
    // assigned, otherwise, assigns a new DOMNodeID and return that.
    public DOMNodeId GetDomNodeId()
    {
        throw new NotImplementedException();
    }

    public override string InterfaceName => throw new NotImplementedException();

    public override ExecutionContext? GetExecutionContext()
    {
        throw new NotImplementedException();
    }
}
