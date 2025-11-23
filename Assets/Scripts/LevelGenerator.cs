using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    /* Idea:
    
    - The level will be generated into a matrix of mxn size, occupied by RoomNodes (or their gameobjects).
    - The information contained in the RoomNodes that occupy the cells of this structure will be used
    to generate the real level. The information they need to contain is the directions in which the room is connected.
    - The positions to place each real room is determined based on the cell position and the scale of each room.
    The scale is assumed to be uniform for each room. The position is on a [column][row] basis.


    For the sake of debugging, this class will include a method called UpdateVisuals() that will iterate
    over the level datastructure, move the transform of RoomNode placeholders into place (and scale appropraitely),
    and update their visuals to match.

    The user must be able to specify prefab to be used at each type of level. So the pick-and-place algorithm must
    look these up according to the connectivity of each room.



    Main loop:

    Initialize starting room

    find room that can be expanded from
    do expansion
    update connectivities of rooms accordingly
    repeat until enough rooms added / termination condition reached. 

    then, do pick and place.
    But for now, just iterate over each non-null cell and transform it to the correct location

    maybe to make it easy just instantiate every cell and update connectivites as required. They can later be purged if not needed. :))))))))

    */
}
