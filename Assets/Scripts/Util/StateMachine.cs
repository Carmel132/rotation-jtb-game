using System.Collections.Generic;
using System;
#nullable enable
public class StateMachine<NodeType>
{
    internal struct Node
    {
        public NodeType type;
        public Action? onEnter, onExit, onLoop;
    }

    internal struct Arrow
    {
        public NodeType target;
        public Func<bool> condition;
    }


    Dictionary<NodeType, Node> nodes;
    Dictionary<NodeType, List<Arrow>> graph;
    Node currentNode;
    

    public void loop()
    {
        currentNode.onLoop?.Invoke();
        if (!graph.ContainsKey(currentNode.type)) { return; }
        foreach (Arrow arrow in graph[currentNode.type])
        {
            if (arrow.condition.Invoke())
            {
                currentNode.onExit?.Invoke();
                currentNode = nodes[arrow.target];
                currentNode.onEnter?.Invoke();
            }
        }
    }

    public void addNode(NodeType node, Action? onEnter, Action? onExit, Action? onLoop)
    {
        nodes[node] = new Node { type = node, onEnter = onEnter, onExit = onExit, onLoop = onLoop };
    }

    public void addArrow(NodeType source, NodeType target, Func<bool> condition)
    {
        Arrow newArrow = new Arrow { target = target, condition = condition };
        if (graph.ContainsKey(source))
        {
            graph[source].Add(newArrow);
        }
        else
        {
            graph[source] = new List<Arrow> { newArrow };
        }
    }

    public void setCurrentNode(NodeType node)
    {
        currentNode = nodes[node];
    }

    public StateMachine()
    {
        nodes = new Dictionary<NodeType, Node>();
        graph = new Dictionary<NodeType, List<Arrow>>();
    }

    public NodeType getCurrentNode()
    {
        return currentNode.type;
    }
}