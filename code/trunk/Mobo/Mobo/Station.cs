/**
* Author: Christopher Cola
* Created on 28/01/2016
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;

namespace Mobo
{

    class Station
    {
        public int id;

        public StationSpawner spawner;

        public Vector2 position;

        HashSet<Vector2> vectors = new HashSet<Vector2>();

        // Station generation core variables
        public int max_depth;
        public int inital_branches;

        // Shouldn't be changed except for testing purposes
        // Defines the depth at which branches occur
        public int split_depth = 6;

        // The two primary data structures
        // A tree that defines the structure of the station
        // and the 2D array in which the station is built
        public Node station_tree;

        // Flattened tree in a list
        public ConcurrentDictionary<int, Node> flat_list = new ConcurrentDictionary<int, Node>();

        Random rand = new Random(Guid.NewGuid().GetHashCode());

        public string debug_string;
        public int debug_size;

        public Station(Vector2 start_position, int max_depth, int difficulty, int initial_branches, StationSpawner spawner)
        {
            this.id = new Random(Guid.NewGuid().GetHashCode()).Next();
            this.position = start_position;
            this.max_depth = max_depth;
            this.inital_branches = initial_branches;
            this.spawner = spawner;

            // Initialise the station tree
            station_tree = new Node(0, null);
            station_tree.data = new StationNode(station_tree, this);
            station_tree.position = Vector2.Zero;
            vectors.Add(Vector2.Zero);

            // Generate the initial tree
            GenerateNodeChildren(station_tree, 1, difficulty);

            // For each initial node, recursively build branch
            foreach (Node child in station_tree.children)
            {
                BuildStation(child);
            }

            foreach(Node child in station_tree.children.ToArray())
            {
                CleanTree(station_tree);
            }

            Initialise();
        }

        public Station(Node imported_tree, Vector2 position)
        {
            station_tree = imported_tree;
            this.position = position;
            this.spawner = Online.spawner;

            Initialise();
        }

        public void Initialise()
        {
            RecursiveBuild(station_tree);

            AssignNodeTextures();

            debug_string = station_tree.Print(0);
            debug_size = station_tree.Count();
        }

        public void RecursiveBuild(Node node)
        {
            flat_list.TryAdd(node.id, node);

            foreach (Node child in node.children)
            {
                RecursiveBuild(child);
            }
        }

        private void CleanTree(Node node)
        {
            foreach (Node child in node.children.ToArray())
            {
                CleanTree(child);
            }

            if (node.data == null)
            {
                node.parent.Remove(node);
            }

            else if (node.data.nodeType == StationNodeType.Pipe && node.children.Count == 0)
            {
                node.parent.Remove(node);
            }
        }

        private int GenerateNumNodes(int difficulty, int depth)
        {
            // The number of initial nodes is not randomly generated
            // It is instead based on the input initial branches var
            if (depth == 1)
            {
                return inital_branches;
            }
            // Every 5 (customisable) nodes, calculate 1-3 branches
            else if (depth % split_depth == 0)
            {
                if (rand.Next(1, 101) <= 10 * difficulty)
                {
                    return 3;
                }
                else if (rand.Next(1, 101) <= 20 * difficulty)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            // Otherwise continue a single chain of nodes (pipes)
            else return 1;
        }

        private Node GenerateNodeChildren(Node node, int depth, int difficulty)
        {
            int num_nodes = GenerateNumNodes(difficulty, depth);

            if (depth <= max_depth)
            {
                for (int i = 0; i < num_nodes; i++)
                {
                    Node to_add = GenerateNodeChildren(new Node(depth, node), depth + 1, difficulty);
                    node.Add(to_add);
                }

            }

            return node;
        }

        private void BuildStation(Node node)
        {
            bool valid = false;

            HashSet<Vector2> poss = new HashSet<Vector2>();
            poss.Add(new Vector2(1, 0));
            poss.Add(new Vector2(-1, 0));
            poss.Add(new Vector2(0, 1));
            poss.Add(new Vector2(0, -1));

            List<Vector2> shuff = poss.OrderBy(x => rand.Next()).ToList();

            // We keep looping if we fail to find a good spot to build a node
            // If we fail at this 20 times we give up, to be cleaned later
            while (!valid && shuff.Count > 0)
            {
                // We start building where our parent is, this allows us to branch
                // at the right locations
                Vector2 new_build_pos = node.parent.position;

                // Generate a random direction out of four (N,E,S,W)
                new_build_pos += shuff[0];

                // We now make sure that the build position is currently occupied
                // And that there isn't already a node built in the place we want to
                // If it's all okay, we build the node and update the position
                if (!vectors.Contains(new_build_pos))
                {
                    valid = true;
                    node.position = new_build_pos;
                    node.data = new StationNode(node, this);
                    vectors.Add(new_build_pos);
                }

                shuff.RemoveAt(0);
            }

            // Recursively build the rest of the tree
            foreach (Node child in node.children)
            {
                BuildStation(child);
            }
        }

        private void AssignNodeTextures()
        {
            foreach (Node node in flat_list.Values)
            {
                StationNode snode = node.data;

                if (snode.nodeType == StationNodeType.Pipe)
                {
                    string k = "nswe";                      

                    Vector2 diff = node.parent.position - node.position;

                    if (diff.X == 1) k = k.Replace('s', 'S');
                    if (diff.X == -1) k = k.Replace('n', 'N');
                    if (diff.Y == 1) k = k.Replace('e', 'E');
                    if (diff.Y == -1) k = k.Replace('w', 'W');

                    foreach (Node child in node.children)
                    {
                        Vector2 childiff = child.position - node.position;

                        if (childiff.X == 1) k = k.Replace('s', 'S');
                        if (childiff.X == -1) k = k.Replace('n', 'N');
                        if (childiff.Y == 1) k = k.Replace('e', 'E');
                        if (childiff.Y == -1) k = k.Replace('w', 'W');
                    }

                    switch (k)
                    {
                        case "NSwe": snode.texture = ContentStore.pipe_h; break;
                        case "nsWE": snode.texture = ContentStore.pipe_v; break;

                        case "NsWe": snode.texture = ContentStore.pipe_se; break;
                        case "NswE": snode.texture = ContentStore.pipe_ne; break;
                        case "nSWe": snode.texture = ContentStore.pipe_sw; break;
                        case "nSwE": snode.texture = ContentStore.pipe_nw; break;

                        case "NsWE": snode.texture = ContentStore.pipe_e; break;
                        case "nSWE": snode.texture = ContentStore.pipe_w; break;
                        case "NSWe": snode.texture = ContentStore.pipe_s; break;
                        case "NSwE": snode.texture = ContentStore.pipe_n; break;

                        case "NSWE": snode.texture = ContentStore.pipe_c; break;
                    }
                }

                else if (snode.nodeType == StationNodeType.Turret)
                {
                    snode.texture = ContentStore.orb_blue;
                }

                snode.origin = new Vector2(snode.texture.Width / 2, snode.texture.Height / 2);
            }
        }

        public void Update()
        {
            foreach (Node node in flat_list.Values)
            {
                if(flat_list.ContainsKey(node.id))
                {
                    node.data.Update();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Node node in flat_list.Values)
            {
                if (flat_list.ContainsKey(node.id))
                {
                    node.data.Draw(spriteBatch);
                }
            }
        }
    }
}