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
            //For checking whether the player is near town npcs or not.
            bool checkNearbyTownNpcs = false;
            //Player is active, there's no delay for event and player's Y position must not be deeper than 67.5 tiles below the surface.
            if ((args.Player!.active && Main.invasionDelay == 0 &&
                (double)args.Player.position.Y < Main.worldSurface * 16 + (double)NPC.sHeight) || Main.remixWorld)
            {
                int invasionRange = 3000;
                //Vanilla event handling.
                if (Main.invasionType > 0 && Main.invasionSize > 0)
                {
                    //Checks if the player is in the idea invasion range for spawning invasion npc.
                    //Note that events like the Goblin invasion uses invasionX to approach to the ~center of the world,
                    //but if the player happens to be in their way of approaching, the invasion npc will still be spawned.
                    if ((double)args.Player.position.X > Main.invasionX * 16.0 - (double)invasionRange &&
                    (double)args.Player.position.X < Main.invasionX * 16.0 + (double)invasionRange)
                    {
                        //Enables event flag, tell Terraria that there does have an event going on.
                        //By doing this, Terraria will run into the piece of code that only spawns event npc.
                        args.AnyOngoingEvent = true;
                    }
                    //If the invasion arrives near the horizontal world center, invasion npc can also be spawned by player being near of any town npc.
                    else if (Main.invasionX >= (double)(Main.maxTilesX / 2 - 5) && Main.invasionX <= (double)(Main.maxTilesX / 2 + 5))
                    {
                        checkNearbyTownNpcs = true;
                    }
                }
                //Modded event handling, pretty much the same as the vanilla one.
                /*
                 We handle this seperately because there's a bug in last version when you try to spawn a vanilla event immediately after a custom event has ended,
                 the vanilla event will spawn its enemies immediately without giving players time to prepare.
                */
                else if (Core.eventType > 0 && Core.eventSize > 0)
                {
                    if ((double)args.Player.position.X > Core.eventX * 16.0 - (double)invasionRange &&
                    (double)args.Player.position.X < Core.eventX * 16.0 + (double)invasionRange)
                    {
                        args.AnyOngoingEvent = true;
                    }
                    else if (Core.eventX >= (double)(Main.maxTilesX / 2 - 5) && Core.eventX <= (double)(Main.maxTilesX / 2 + 5))
                    {
                        checkNearbyTownNpcs = true;
                    }
                }

                if (checkNearbyTownNpcs)
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
            //Disables vanilla event handling because we modified it to also include a handler for custom event.
            args.Handled = true;
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
                if (evins.EventID == args.EventID)
                {
                    evins.ConfigureEvent(args.EventID);
                    //Only if the value of InfiniteInvasion in config.json is true.
                    //This would make the invasion last indefinitely.
                    if (TShockAPI.TShock.Config.Settings.InfiniteInvasion)
                    {
                        Core.eventSize = 20_000_000;
                    }
                    Core.eventSizeStart = Core.eventSize;
                    args.Handled = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Called each time there's an announcement message that's not from vanilla.
        /// Adds additional code for handling custom event.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.ModdedEventAnnouncementEventArgs"/></param>
        internal static void MyHook_OnModdedEventAnnouncement(object? sender, ExHooks.Events.Invasion.ModdedEventAnnouncementEventArgs args)
        {
            //There's no need to check whether the announcement message is empty or not since the hook has already done it for us.
            if (Core.eventSize <= 0) //The event has been defeated.
            {
                foreach (var evins in Core.eventInstances!)
                {
                    if (evins.EventID == Core.eventType)
                    {
                        args.AnnouncementMsg = evins.DefeatedMsg.Key;
                        args.MsgR = evins.DefeatedMsg.Value.R;
                        args.MsgG = evins.DefeatedMsg.Value.G;
                        args.MsgB = evins.DefeatedMsg.Value.B;
                        break;
                    }
                }
            }
            else if (Core.eventX < (double)Main.spawnTileX) //The event is approaching from the west
            {
                foreach (var evins in Core.eventInstances!)
                {
                    if (evins.EventID == Core.eventType)
                    {
                        args.AnnouncementMsg = evins.WestIncomingMsg.Key;
                        args.MsgR = evins.WestIncomingMsg.Value.R;
                        args.MsgG = evins.WestIncomingMsg.Value.G;
                        args.MsgB = evins.WestIncomingMsg.Value.B;
                        break;
                    }
                }
            }
            else if (Core.eventX > (double)Main.spawnTileX) //The event is approaching from the east.
            {
                foreach (var evins in Core.eventInstances!)
                {
                    if (evins.EventID == Core.eventType)
                    {
                        args.AnnouncementMsg = evins.EastIncomingMsg.Key;
                        args.MsgR = evins.EastIncomingMsg.Value.R;
                        args.MsgG = evins.EastIncomingMsg.Value.G;
                        args.MsgB = evins.EastIncomingMsg.Value.B;
                        break;
                    }
                }
            }
            else //The event has arrived!
            {
                foreach (var evins in Core.eventInstances!)
                {
                    if (evins.EventID == Core.eventType)
                    {
                        args.AnnouncementMsg = evins.ArrivedMsg.Key;
                        args.MsgR = evins.ArrivedMsg.Value.R;
                        args.MsgG = evins.ArrivedMsg.Value.G;
                        args.MsgB = evins.ArrivedMsg.Value.B;
                        break;
                    }
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
                    if (evins.EventID == Core.eventType && args.Player!.active)
                    {
                        args.Handled = evins.SpawnEventNPC(args.Player, args.TileX, args.TileY);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Called each time an npc is killed (after <see cref="OTAPI.Hooks.NPC.Killed"/>).
        /// Reduces the on-going custom event size if the npc belongs to that event.
        /// </summary>
        /// <param name="obj">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.PostEventNpcKilledEventArgs"/></param>
        internal static void MyHook_OnPostEventNPCKilled(object? obj, ExHooks.Events.Invasion.PostEventNpcKilledEventArgs args)
        {
            //Since events created this way may share duplicate npc between custom events so we are gonna rely on the currently active event.
            foreach (var evins in Core.eventInstances!)
            {
                if (evins.EventID == Core.eventType)
                {
                    if (evins.EnemyPool.ContainsKey(args.NPC!.type))
                    {
                        var enemySize = evins.EnemyPool[args.NPC.type];
                        if (enemySize > 0)
                        {
                            Core.eventSize -= enemySize;
                            if (Core.eventSize < 0)
                            {
                                Core.eventSize = 0;
                            }
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Called on every game ticks to update the current on-going event.
        /// Provides support for updating custom event as well.
        /// </summary>
        /// <param name="obj">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.PostEventUpdateEventArgs"/></param>
        internal static void MyHook_OnPostEventUpdate(object? obj, ExHooks.Events.Invasion.PostEventUpdateEventArgs args)
        {
            if (Core.eventType > 0)
            {
                if (Core.eventSize <= 0)
                {
                    foreach (var evins in Core.eventInstances!)
                    {
                        if (evins.EventID == Core.eventType)
                        {
                            evins.OnEventCompletion();
                            break;
                        }
                    }
                    Main.InvasionWarning();
                    Core.eventType = 0;
                    Main.invasionDelay = 0;
                }
                if (Core.eventX != (double)Main.spawnTileX)
                {
                    float evSpeed = (float)Main.dayRate;
                    if (evSpeed < 1f)
                    {
                        evSpeed = 1f;
                    } 
                    if (Core.eventX > (double)Main.spawnTileX)
                    {
                        Core.eventX -= (double)evSpeed;
                        if (Core.eventX <= (double)Main.spawnTileX)
                        {
                            Core.eventX = (double)Main.spawnTileX;
                            Main.InvasionWarning();
                        }
                        else if (evSpeed > 0f)
                        {
                            Core.eventWarn--;
                        }
                    }
                    else if (Core.eventX < (double)Main.spawnTileX)
                    {
                        Core.eventX += evSpeed;
                        if (Core.eventX >= (double)Main.spawnTileX)
                        {
                            Core.eventX = (double)Main.spawnTileX;
                            Main.InvasionWarning();
                        }
                        else if (evSpeed > 0f)
                        {
                            Core.eventWarn--;
                        }
                    }
                    if (Core.eventWarn <= 0)
                    {
                        Core.eventWarn = 3600;
                        Main.InvasionWarning();
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
            foreach (var devent in Core.eventInstances!.OfType<IDaytimeEvent>())
            {
                args.Handled = devent.CheckRequirementsForDaytimeEvent();
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
            foreach (var nevent in Core.eventInstances!.OfType<INighttimeEvent>())
            {
                args.Handled = nevent.CheckRequirementsForNighttimeEvent();
            }
        }
    }
}
