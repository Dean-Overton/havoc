# Havoc  -  *By Noughty Nemesis*

| Collaborators |
| :-----------: |
| Dean Overton |
| Brandon Emre Ozdemir |
| Teek |
| Steve |
| Mayowa Adeniyi |

## How to play the game
* Make sure you are on the `Main Menu` scene: `Assets -> Scenes -> Main Menu`
* Then click the `New Game` button
* WASD to move, right-click to dash, left-click to shoot

## Making changes to Waves/Levels

* In the Unity Inspector scene hierachy, select the `GameController` object
* Find the `Level Controller` script component
* Expand the `Levels` drop down field
* You should see 4 Elements total, this is for our 3 levels for the prototype, each level/element represents a playable island
    * Element 0 - The initial level at the spawn island, **this must have `0` waves, no enemies or waves on this island**
    * Element 1 - The first `tutorial` level/island. 
    * Element 2 - The second `tree` level/island. 
    * Element 3 - The third `cave` level/island.
* Each element has a `Waves` drop down. You can set the number of waves the player must beat
* There will now be an element for each wave in the `Waves` drop down. Each wave/element has an `Enemies` and `Spawn Points` field
    * Select the number of enemies you want to spawn in this wave
    * You probably should have the same number of spawn points for each enemy
    * For each enemy, select an enemy prefab from the `Assets -> Prefabs -> Enemies` folder
    * For each spawn point:
        * In the scene hierachy, under `_Environment_`, then the curresponding `_Level_`
        * You should see a `_Spawnpoints_` parent object, under that object, there should be some empty game objects with a transform
        * You can select one of these empty game objects as a spawn point, or feel free to make your own
        * Probably have a different spawn point for each feild, otherwise enemies spawn and go flying

## Technical Game Structure

The level begins when the player collides with one of the `_WaveTriggers_` child objects.\
The next wave begins when the last enemy is killed in the last wave.\
When the last wave has been completed, the bridge to the next island materialises out of thin air. 


# GitHub Flow/Collaboration
1. Create a Branch
- Always create a new branch for each feature or bugfix. This keeps your work separate from the main codebase.
- ```git switch -c feature-branch-name```

2. Make Changes. Make a SEPERATE scene to work on your feature. At the end, we will merge the scenes together!
3. Commit Changes: Commit them with a meaningful message.

- ```git add .``` (stage all changes)

- ```git status``` (to check what is staged/about to be committed)

- ```git commit -m "Brief description of the changes"```

4. Push Changes to GitHub
- ```git push origin feature-branch-name```

5. Create a Pull Request: Create a pull request to merge your changes into the main branch. (main)
- Click the "Compare & pull request" button.
- Provide a title and description for your pull request.
- Click "Create pull request".
- If your pull request says unable to merge, run ```git merge main``` while on your feature branch to bring the new changes back into your feature.

6. Review and Merge
- Your pull request will be reviewed by your teammates. Once approved, it can be merged into the main repository.

### Tips
Keep your branches up-to-date: Regularly merge the latest changes from the main branch to avoid conflicts.

- ```git fetch upstream```
- ```git checkout main```
- ```git merge upstream/main```
- Work in small increments: Smaller changes are easier to review and less likely to cause conflicts.
