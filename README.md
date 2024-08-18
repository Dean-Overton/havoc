## GitHub Flow/Collaboration
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
