import networkx as nx
import matplotlib.pyplot as plt
import xml.etree.ElementTree as ET

def hierarchy_pos(G, root, width=2000, vert_gap = 200, vert_loc = 0, xcenter = 0.5,
                  pos = None, parent = None):
    '''If there is a cycle that is reachable from root, then this will see infinite recursion.
       G: the graph
       root: the root node of current branch
       width: horizontal space allocated for this branch - avoids overlap with other branches
       vert_gap: gap between levels of hierarchy
       vert_loc: vertical location of root
       xcenter: horizontal location of root
       pos: a dict saying where all nodes go if they have been assigned
       parent: parent of this branch.'''
    if pos == None:
        pos = {root:(xcenter,vert_loc)}
    else:
        pos[root] = (xcenter, vert_loc)
    neighbors = G.neighbors(root)
    if parent != None:
        neighbors.remove(parent)
    if len(neighbors)!=0:
        dx = width/len(neighbors)
        nextx = xcenter - width/2 - dx/2
        for neighbor in neighbors:
            nextx += dx
            pos = hierarchy_pos(G,neighbor, width = dx, vert_gap = vert_gap,
                                vert_loc = vert_loc-vert_gap, xcenter=nextx, pos=pos,
                                parent = root)
    return pos


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


labels = {}
g = nx.Graph()
for node in nodes:
    g.add_node(node.id)
    labels[node.id] = node.label

for relation in relations:
    g.add_edge(relation.start, relation.end)

#g.add_edge('a','b', weight=0.1)

pos = hierarchy_pos(g,1,vert_gap = 20)
#pos = nx.spring_layout(g)
plt.figure(figsize=(80,80))
plt.axis('equal')




nx.draw(g,pos=pos, with_labels=False, node_size=200, node_color='#A0CBE2')
nx.draw_networkx_labels(g,pos, labels)
nx.draw_networkx_edge_labels(g, pos=pos)
plt.savefig('circular_tree.png')








