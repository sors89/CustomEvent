using Terraria;

namespace CustomEvent
{
    internal static class Handler
    {
        /// <summary>
        /// Called each time a new event is about to happen.
        /// Adds an additional check that resets the custom event type if the previous event has ended.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.EventConfigurationEventArgs"/></param>
        internal static void MyHook_OnEventPreparation(object? sender, ExHooks.Events.Invasion.EventPreparationEventArgs args)
        {
            if (Core.eventType != 0 && Core.eventSize == 0)
            {
                Core.eventType = 0;
            }
        }

        /// <summary>
        /// Called each time a new npc is about to be spawned.
        /// Replaces vanilla's event flag check behavior with a new one that supports both vanilla and custom event.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.EventFlagCheckEventArgs"/>.</param>
        internal static void MyHook_OnEventFlagCheck(object? sender, ExHooks.Events.Invasion.EventFlagCheckEventArgs args)
        {
            if (args.Player == null)
            {
                return;
            }

            //Checks if there is any vanilla or custom event going on including npc in that event can still be spawned and the player's Y position must not be deeper than 67.5 tiles below the surface.
            if (args.Player.active && (Main.invasionType > 0 || Core.eventType > 0) && (Main.invasionDelay == 0 || Core.eventDelay == 0) && 
                (Main.invasionSize > 0 || Core.eventSize > 0) && (double)args.Player.position.Y < Main.worldSurface * 16.0 + (double)NPC.sHeight || Main.remixWorld)
            {
                int invasionRange = 3000;

                //Checks if the player is in the idea invasion range for spawning invasion npc.
                //Note that events like the Goblin invasion uses invasionX to approach to the ~center of the world,
                //but if the player happens to be in their way of approaching, the invasion npc will still be spawned.
                if (((double)args.Player.position.X > Main.invasionX * 16.0 - (double)invasionRange &&
                    (double)args.Player.position.X < Main.invasionX * 16.0 + (double)invasionRange) ||
                    ((double)args.Player.position.X > Core.eventX * 16.0 - (double)invasionRange &&
                    (double)args.Player.position.X < Core.eventX * 16.0 + (double)invasionRange))
                {
                    //Enables event flag, tell Terraria that there does have an event going on.
                    //By doing this, Terraria will run into the piece of code that only spawns event npc.
                    args.AnyOngoingEvent = true;
                }
                //If the invasion arrives near the horizontal world center, invasion npc can also be spawned by player being near of any town npc
                else if (Main.invasionX >= (double)(Main.maxTilesX / 2 - 5) && Main.invasionX <= (double)(Main.maxTilesX / 2 + 5) ||
                        (Core.eventX >= (double)(Main.maxTilesX / 2 - 5) && Core.eventX <= (double)(Main.maxTilesX / 2 + 5)))
                {
                    foreach (var npc in Main.npc)
                    {
                        //Checks if the player is within 187.5 tiles horizontally of any town npc
                        if (npc.townNPC && Math.Abs(args.Player.position.X - npc.Center.X) < (float)invasionRange)
                        {
                            //67% chance to enable event flag which will spawn invasion npc.
                            //The invasion npc spawn rate will be reduced by enabling event flag this way.
                            if (Main.rand.Next(3) != 0)
                            {
                                args.AnyOngoingEvent = true;
                                break;
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called each time an event is about to be happened.
        /// Configures custom event properties
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.EventConfigurationEventArgs"/>.</param>
        internal static void MyHook_OnEventConfiguration(object? sender, ExHooks.Events.Invasion.EventConfigurationEventArgs args)
        {
            foreach (var evins in Core.eventInstances!)
            {
                var evId = (int)evins!.GetType().GetProperty("EventID")!.GetValue(evins)!;
                if (evId == args.EventID)
                {
                    var parameters = new object[] { args.EventID, args.Handled };
                    evins!.GetType().GetMethod("ConfigureEvent")!.Invoke(evins, parameters);
                    args.Handled = (bool)parameters[1];
                }
            }
        }

        /// <summary>
        /// Called each time a new npc is about to be spawned.
        /// Gets on-going custom event to spawn its npc.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.SpawnEventNPCEventArgs"/>.</param>
        internal static void MyHook_OnSpawnEventNPC(object? sender, ExHooks.Events.Invasion.SpawnEventNPCEventArgs args)
        {
            if (Core.eventType != 0 && Core.eventSize > 0)
            {
                foreach (var evins in Core.eventInstances!)
                {
                    var evId = (int)evins!.GetType().GetProperty("EventID")!.GetValue(evins)!;
                    if (evId == Core.eventType && args.Player != null)
                    {
                        var parameters = new object[] { args.Player, args.TileX, args.TileY, args.Handled };
                        evins.GetType().GetMethod("SpawnEventNPC")!.Invoke(evins, parameters);
                        args.Handled = (bool)parameters[3];
                    }
                }
            }
        }

        /// <summary>
        /// Called each time a new day started (at 4:30 am)
        /// Gets all custom events to check its event condition.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.World.CheckDaytimeEventRequirementsEventArgs"/>.</param>
        internal static void MyHook_OnCheckDaytimeEventRequirements(object? sender, ExHooks.Events.World.CheckDaytimeEventRequirementsEventArgs args)
        {
            foreach (var evins in Core.eventInstances!)
            {
                var parameters = new object[] { args.Handled };
                evins!.GetType().GetMethod("CheckRequirementsForDaytimeEvent")!.Invoke(evins, parameters);
                args.Handled = (bool)parameters[0];
            }
        }

        /// <summary>
        /// Called each time a new night started (at 7:30 pm)
        /// Gets all custom events to check its event condition.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.World.CheckNighttimeEventRequirementsEventArgs"/>.</param>
        internal static void MyHook_OnCheckNighttimeEventRequirements(object? sender, ExHooks.Events.World.CheckNighttimeEventRequirementsEventArgs args)
        {
            foreach (var evins in Core.eventInstances!)
            {
                var parameters = new object[] { args.Handled };
                evins!.GetType().GetMethod("CheckRequirementsForNighttimeEvent")!.Invoke(evins, parameters);
                args.Handled = (bool)parameters[0];
            }
        }
    }
}
