using System.Collections.Generic;
using UnityEngine;
public static class AIDictionaty 
{
    public static string GetText(string key)
    {
        if (bd.ContainsKey(key))
        {
            Debug.Log("AIDictionaty key: " + key + ": " + bd[key]);
            return bd[key];
        }
        else
        {
            Debug.Log("AIDictionaty: Not conteins key: " + key);
            return "";
        }
            
    }

    public static IDictionary<string, string> bd = new Dictionary<string, string>()
        {
            
            //Episode 1
            { "en-US-C_ai_ep1_e1",
                "The connection's been established. A neural activity has been detected. Launching the regeneration system"+
                "Low energy level of life support system. Charging is required. Scanning the space for energy resources." +
                "The energy source has been detected. It’s a light fighter spaceship."+
                "Navigation system is online. Launching the acceleration. Continue moving toward to the energy source"},
            {"en-US-C_ai_ep1_faraway",
                "Energy is running out, сhange the motion vector to the target" },
            {"en-US-C_ai_ep1_deadEnergyLevel",
                "Energy system is shutting down." },
            {"en-US-C_ai_ep1_a1",
                "The systems of the ship have successfully been launched " +
                "Move through the gate to calibrate the navigation systems."},
            {"en-US-C_ai_ep1_a2a",
                "Everything goes well. Just a few more ahead." },

            {"en-US-C_ai_ep1_a2b",
                "The navigation system has successfully been calibrated. " +
                "The scanning system of the nearest space is being done. " +
                "Scanning is not possible. Looks like there is a device nearby that blocks all subspace signals in this area. Calculating coordinates." +
                "Coordinates have been found. " +
                "You must destroy this jammer to terminate the interference. " },
            
            {"en-US-C_ai_ep1_a3",
                "Discovered multiple energy sources. Scanning... " +
                "3 space drones have been found. They are moving in our direction. "+
                 "The nearest enemy will be marked on the compass." },
            
            {"en-US-C_ai_ep1_a5",
                "All enemies have been destroyed. Scanning the space. " +
                "The wreckage of an ancient fighter plane has been found. You need to get closer to them, " +
                "for more detailed scanning." },
            
            {"en-US-C_ai_ep1_a6",
                "Scanning is complete.  It looks like there was a clash of two warring factions here... " +
                "The energy signature shows the presence of another ship that is different from this. " +
                "The target is marked on the compass. " },
            {"en-US-C_ai_ep1_a7",
                "The weak energy radiation has been detected emanating from the transport ship. " +
                "It looks like the warp core of this ship was not damaged during the battle. " +
                "It is necessary to dock and analyze the systems of the ship. " },
            //Episode 2.1
            {"en-US-C_ai_ep2_1_c8",
                "The ship is fully functional, but there is not enough energy to make a warp jump. " +
                "It looks like this ship uses antimatter as its energy. " },
            {"en-US-C_ai_ep2_1_c9",
                "Attention. Fixing the approach of hostile drones. " +
                "An urgent evacuation is required." },
            {"en-US-C_ai_ep2_1_c10",
                "The passage is blocked. " +
                "Proceed to the rescue shuttle." },
            {"en-US-C_ai_ep2_1_c11",
                "Fixing the source of antimatter, plotting a route." },
            //Episode 2
            {"en-US-C_ai_ep2_m1","Our scanner has detected some energy cells on this space station. These cells can be used to charge our ship. You need to figure out how to get them.  When you get closer to a cell, your scanner will detect the antimatter field.  After we have installed all the cells, our ship must be called."},
            {"en-US-C_ai_ep2_e1", "Great! You have found an energy cell.  Use your gravity gun to get it."},
            {"en-US-C_ai_ep2_e2","Good, you have found another cell. Looks like it is inside this antimatter capsule. You need to use your weapon to destroy the code pannel."},
            {"en-US-C_ai_ep2_e3","This cells is protected by the energy shield. It's powering the air on the station.  To get the cell out of the shield you need to hack this terminal by using a special device. It must somewhere on the station. "},
            {"en-US-C_ai_ep2_e3_1","You have found one more cell, but if you use the terminal right now you will disable the air compression system. Then all the air on this station won't be enable any more. I recommend you to find the other cells before taking this one."},
            {"en-US-C_ai_ep2_e3_2","Be careful, after turning the air system off the atmosphere on the station will not be available any more."},
            {"en-US-C_ai_ep2_e3_3","The atmosphere is not available.  The inner system of oxygen delivery is on. The supply of the oxygen will be exhausted in 30 seconds. To fill your oxygen supply find an oxygen tank."},
            {"en-US-C_ai_ep2_e4","The oxygen level is full."},
            {"en-US-C_ai_ep2_e5","You have found the transmitter which can be used to call our ship."},
            {"en-US-C_ai_ep2_e6","It's too dangerous to call our ship.  We need to power up the energy crane by 100 per cent."},
            {"en-US-C_ai_ep2_e_b1","The crane is powered by 33 per cent."},
            {"en-US-C_ai_ep2_e_b2","The crane is powered by 66 per cent."},
            {"en-US-C_ai_ep2_e_b3","The crane is powered by 100 per cent.  We must call our ship. To do it you must use the transmitter you have found earlier."},
            {"en-US-C_ai_ep2_e7","The ship has arrived. Use the terminal on the energy crane to start energy transmitting."},
            {"en-US-C_ai_ep2_e8","My scanner has detected arriving enemies.  Protect the energy crane while the ship is being charged. "},
            {"en-US-C_ai_ep2_e9","The energy level is full. Get out of here!"},
            {"en-US-C_ai_ep2_e10","Dangerous oxygen level!" },
            {"en-US-C_ai_ep2_e11","You failed mission !" },
        };
}
