﻿namespace MapEditorReborn.Commands
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Mirror;
    using RemoteAdmin;
    using UnityEngine;

    /// <summary>
    /// Command used for modifing object's scale.
    /// </summary>
    public class Scale : ICommand
    {
        /// <inheritdoc/>
        public string Command => "scale";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "scl" };

        /// <inheritdoc/>
        public string Description => "Modifies object's scale.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "\nUsage:\n";
                response += "mp scale set (x) (y) (z)\n";
                response += "mp scale add (x) (y) (z)\n";
                return false;
            }

            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            if (!player.TryGetSessionVariable(Handler.SelectedObjectSessionVarName, out GameObject gameObject) || gameObject == null)
            {
                response = "You haven't selected any object!";
                return false;
            }

            if (gameObject.name == "PlayerSpawnPointObject(Clone)")
            {
                response = "You can't modify this object's scale!";
                return false;
            }

            Vector3 newScale;

            switch (arguments.At(0).ToUpper())
            {
                case "SET":
                    {
                        if (arguments.Count < 4)
                        {
                            response = "You need to provide all X Y Z arguments!";
                            return false;
                        }

                        if (float.TryParse(arguments.At(1), out float x) && float.TryParse(arguments.At(2), out float y) && float.TryParse(arguments.At(3), out float z))
                        {
                            newScale = new Vector3(x, y, z);

                            NetworkServer.UnSpawn(gameObject);
                            gameObject.transform.localScale = newScale;
                            NetworkServer.Spawn(gameObject);

                            response = newScale.ToString();
                            break;
                        }

                        response = "Invalid values.";
                        return false;
                    }

                case "ADD":
                    {
                        if (arguments.Count < 4)
                        {
                            response = "You need to provide all X Y Z arguments!";
                            return false;
                        }

                        if (float.TryParse(arguments.At(1), out float x) && float.TryParse(arguments.At(2), out float y) && float.TryParse(arguments.At(3), out float z))
                        {
                            newScale = new Vector3(x, y, z);

                            NetworkServer.UnSpawn(gameObject);
                            gameObject.transform.localScale += newScale;
                            NetworkServer.Spawn(gameObject);

                            response = newScale.ToString();
                            break;
                        }

                        response = "Invalid values.";
                        return false;
                    }

                default:
                    {
                        response = "Invalid command option. Use a command without any arugments to get a list of valid options.";
                        return false;
                    }
            }

            gameObject.UpdateObject(player);
            return true;
        }
    }
}
