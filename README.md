# EncryptoidModPack

This is a mod pack that contains 3 main groups of commands(3 Mods).

These are **Tetherport, AdminTetherport, Retrieve/Return Player**. Each of these is independent, so tethers from AdminTetherport will not affect Tetherport for example.

**Tetherport** is a that mod allows you to create "portals" as an admin, which are points that players can teleport to, creating a tether at their current location that they can return to afterwards. This is useful for setting up events or trading hubs on your server.

**AdminTetherport** is a mod that allows an admin to tetherport to an online player, which will create a tether at their original location that they can return to. This is useful for solving support issues on a server, an admin can go fix it and return easily.

*Retrieve/Return Players*
This allows admins to teleport an online player to them. There are two commands, one which will create a tether for the player they can be returned to(useful for events), and the other will not create the tether(useful for helping players, eg. stuck in Admin Core POI). 

# Install

To install, follow the steps to setup a modfolder.path file here: https://github.com/Encryptoid/zucchini-empyrion/blob/main/README.md
Many thanks again to the work I forked for the base zucchini-empyrion mod, it would have taken a lot longer to get there without their code.

All commands below are read by the mod from the Server chat tab.


# !modpack

This will launch a UI detailing all available commands and how to use them, similar to below.

# Tetherport Commands

**!tetherport** or **!ttp** (All Players)

This will launch a UI showing all current portals available. If the player has already tetherported, they will have an UNTETHER record at the top of their list. Using this will teleport the player to their original location and delete the record. Visiting multplie portals will not overwrite the original tether(until it is used and removed). In this image, only admins will see the commands at the bottom.

![image](https://user-images.githubusercontent.com/89423557/162569138-d6c78c96-fdcb-4803-af54-68270da6a548.png)

**!portal-create** (Admin Only)

This will launch a small UI which asks you to input a portal name. Clicking create will make a portal with that name at the admins current location, that can will show up in the **!ttp** list of portals. By default only admins can see the portal.

![image](https://user-images.githubusercontent.com/89423557/162569197-f333715d-a25c-412c-8a50-5adee2018ed3.png)

**!portal-admin** (Admin Only)

In the portal list, portals will have a column, AdminYN=Y or AdminYN=N. If it's Y, only admins can see the portal. Using this command will launch a UI showing all current portals, clicking on one of them will toggle the AdminYN value, and the portals visibility.

![image](https://user-images.githubusercontent.com/89423557/162569282-dde62342-2641-439b-b950-7c5617b8b475.png)


**!portal-delete** (Admin Only)

This will launch a UI with the list of portals. Clicking on one, will bring you to a further confirm delete window, where there you can permanently delete the portal.

![image](https://user-images.githubusercontent.com/89423557/162569453-74b02638-9ee4-4ad8-9c1f-1395037f01cd.png)


# AdminTetherport & Retrieve/Return Commands (Admin Only)

**!admintetherport** or **!attp***

This command will launch a UI showing all online players. Clicking one will create a tether at your current location and teleport you to the player. Your UNTETHER record will show up at the top of the list next time. Unlike **!tetherport**, this command will overwrite your untether if you have one, so untether before using if you want to save the original location.

![image](https://user-images.githubusercontent.com/89423557/162569636-64340659-e572-4799-be35-1b23c97b7ff9.png)


**!retrieve**

This command launches a UI showing all online players. Clicking one will teleport the player to you with no tether created.

**!retrieve-event**

This command launches a UI showing all online players. Clicking one will teleport the player to you and create a tether for that player at their original location.

![image](https://user-images.githubusercontent.com/89423557/162569654-ab0c6547-2cca-46aa-b810-2ff2d5c9b691.png)


**!return**

This command launches a UI showing all the tethers created with **!retrieve-event**. Clicking one will teleport the player to their tether and delete the record.

![image](https://user-images.githubusercontent.com/89423557/162569664-000dee08-a234-4a9a-a7c1-80e6c06b0a63.png)




If you have an questions/issues/suggestions, feel free to search for me, Encryptoid, on the Empyrion discord and message me! I hope this can help server owners small and large. Enjoy!



