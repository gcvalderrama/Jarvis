from ete3 import *
import xml.etree.ElementTree as ET


class RSTNode:
    def __init__(self):
        self.id = None
        self.label = None
    def __str__(self):
        return self.label

class RSTRelation:
    def __init__(self):
        self.start = None
        self.end = None
        self.label = None

tree = ET.parse('LA091790-0041.txt.xml.xml')
root = tree.getroot()
nodes = list()
relations = list()
for token_xml in root.iter('node'):
    node = RSTNode()
    node.id = int(token_xml.get('id'))
    node.label = token_xml.get('label')
    nodes.append(node)

for token_xml in root.iter('relation'):
    relation = RSTRelation()
    relation.start = int(token_xml.get('start'))
    relation.end= int(token_xml.get('end'))
    relation.label = token_xml.get('label')
    relations.append(relation)

def getkey(node):
    return node.end
def recursive(TNode, Node, InNodes, InRelations, level):


    ends = list()

    for relation in InRelations:
        if (Node.id == relation.start):
            ends.append(relation)


    ends.sort(key=getkey)

    for relation in ends:
        for pnode in InNodes:
            if (pnode.id == relation.end):
                n = TreeNode(name = pnode.label)
                TNode.add_child(n)
                recursive(n, pnode, InNodes, InRelations, level+1)





t = Tree()
target = nodes[0]
n = TreeNode(name=target.label)
t.add_child(n)
recursive(n, target, nodes, relations,1)

ts = TreeStyle()
ts.show_leaf_name = False
def my_layout(node):
        F = TextFace(node.name, tight_text=True)
        add_face_to_node(F, node, column=0, position="branch-right")
ts.layout_fn = my_layout
t.show(tree_style=ts)
#print t.get_ascii()