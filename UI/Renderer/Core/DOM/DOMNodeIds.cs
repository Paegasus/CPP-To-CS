namespace UI.Renderer.Core.DOM;

public static class DOMNodeIds
{
    public const int kInvalidDOMNodeId = 0;

    // WeakReference<Node> allows nodes to be garbage collected when no longer
    // referenced elsewhere, matching C++'s WeakMember<Node>.
    private static readonly Dictionary<int, WeakReference<Node>> IdToNodeMap = new();
    private static int last_id_ = 0;

    // Return a DOMNodeId or 0 if one hasn't been assigned.
    public static int ExistingIdForNode(Node? node) => node?.NodeId ?? kInvalidDOMNodeId;

    // Return the existing DOMNodeId if already assigned, otherwise assign a new one and return it.
    public static int IdForNode(Node? node)
    {
        if (node == null)

            return kInvalidDOMNodeId;

        if (node.NodeId == kInvalidDOMNodeId)
        {
            // Wrap around on overflow, matching C++'s WeakIdentifierMap::Next().
            if (last_id_ == int.MaxValue)
                last_id_ = 0;
            
            node.NodeId = ++last_id_;

            IdToNodeMap[node.NodeId] = new WeakReference<Node>(node);
        }

        return node.NodeId;
    }

    // Return a node for the DOMNodeId or null if one hasn't been assigned.
    public static Node? NodeForId(int id)
    {
        if (id == kInvalidDOMNodeId)

            return null;

        if (!IdToNodeMap.TryGetValue(id, out WeakReference<Node>? weakRef))

            return null;

        weakRef.TryGetTarget(out Node? node);
        
        return node;
    }
}
