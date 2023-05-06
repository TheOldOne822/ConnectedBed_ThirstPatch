using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.Sound;
using VNPE;
using DubsBadHygiene;
using zed_0xff.VNPE;

namespace NVPEConnectedBed_ThirstPatch
{
	[StaticConstructorOnStartup]
	static public class HarmonyPatches
	{
		public static Harmony harmonyInstance;


		static HarmonyPatches()
		{
			harmonyInstance = new Harmony("rimworld.rwmods.NVPEConnectedBed_ThirstPatch");
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
		}
	}


	[HarmonyPatch(typeof(Building_ConnectedBed))]
	[HarmonyPatch("TickRare")]
	internal static class Building_ConnectedBedThirst
    {
		public static void Postfix(ref Building_ConnectedBed __instance)
		{
			if (DubsBadHygiene.Settings.ThirstNeed == true)
			{
                // Find if anyone is thirsty
                Building_ConnectedBed Bed = __instance;
						// Find people in the bed
						foreach(Pawn BedPerson in Bed.CurOccupants)
						{
							Need_Thirst BedThirst = BedPerson.needs.TryGetNeed<Need_Thirst>();

							if (BedThirst != null)
							{
								// Are they thirsty?
								if (BedThirst.CurLevelPercentage < 0.5)
								{
									// Is it piped?
									CompPipe PipeProperties = __instance.TryGetComp<CompPipe>();
									if (PipeProperties != null)
									{

										// Can we draw the water?
										ContaminationLevel WaterTaint = ContaminationLevel.Treated;
										if (PipeProperties.pipeNet.PullWater((float)(2.7 * BedThirst.CurLevelPercentage), out WaterTaint) == true)	// People drink 2.7L of water a day
										{
											// Sippy sippy
											//BedPerson.needs.TryGetNeed<Need_Thirst>().Drink();
											BedThirst.Drink(100.0f);
											
											SanitationUtil.ContaminationCheckDrinking(BedPerson, WaterTaint);
										}
									}
								}
							}
						}
			}
		}
	}

}
