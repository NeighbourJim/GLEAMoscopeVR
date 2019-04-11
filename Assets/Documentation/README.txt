GLEAMoscopeVR - Iteration 02 - Prototyping Project
=============================

Project Master Control: {NAME}

Project details...



PROJECT STANDARDS / GUIDELINES
==============================

Textures / Materials:
---------------------
Due to the number of models and textures required in this project, 
the 'Materials' and 'Textures' folders in the root Assets directory will be used sparingly.

- When importing models for any object that may be used as an interactable they are to be placed 
  in the most appropriate folder, within the Interactables folder. Then:
	
	- Select the model in the Project Hierarchy and go to the Materials tab (Inspector Import Settings)
	- Ensure Import Materials and sRGB Albedo Colours boxes are ticked
	- Click Extract Materials to open the Select Materials Folder dialogue:
		- If there is a Materials folder in within the folder containing the model being imported, 
		  select the folder and click "Select Folder"
		- If there is NOT a materials folder within the folder containing the model being imported,
		  click New Folder, name it "Materials" and click "Select Folder".
	- If you have the option to Extract Textures (still in the Inspector Import Settings), 
	  repeat the process for extracting Materials but replace "Materials" with "Textures"
	- If there should be Textures and / or Materials but you are unable to extract them, speak to
	  {Art Dude} or the Media team.
