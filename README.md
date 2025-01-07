In this repository, I am implementing an alternative ECS approach that differs from the chunk-based ECS system I previously published. In this implementation:

Entities are represented as simple IDs.
Components are stored separately in ComponentContainers, organized by the entity index.

For further details, you can read my article discussing the Proof of Concept (POC) of a custom chunk-based ECS system : POC of Custom Chunk-Based ECS:
A Performance Solution with GPU Instancing, Bitwise Archetype Check, Iterative Quadtree Integration, and Multithreaded Iterative A* Pathfinding System
https://medium.com/@kemagedy/poc-of-custom-chunk-based-ecs-4adb260b3cc6
Github : https://github.com/kegedev/chunk-based-ecs-4x-game.git
