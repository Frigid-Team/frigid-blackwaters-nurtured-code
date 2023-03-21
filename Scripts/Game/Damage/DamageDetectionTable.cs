using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public static class DamageDetectionTable
    {
        private static HashSet<int> alignmentInteractionCodes;
        private static HashSet<int> channelInteractionCodes;

        static DamageDetectionTable()
        {
            alignmentInteractionCodes = new HashSet<int>();
            channelInteractionCodes = new HashSet<int>();
            foreach ((DamageAlignment damageAlignment1, DamageAlignment damageAlignment2) alignmentInteraction in GetAlignmentInteractions())
            {
                alignmentInteractionCodes.Add(CalculateAlignmentInteractionCode(alignmentInteraction.damageAlignment1, alignmentInteraction.damageAlignment2));
                alignmentInteractionCodes.Add(CalculateAlignmentInteractionCode(alignmentInteraction.damageAlignment2, alignmentInteraction.damageAlignment1));
            }
            foreach ((DamageChannel damageChannel1, DamageChannel damageChannel2) channelInteraction in GetChannelInteractions())
            {
                channelInteractionCodes.Add(CalculateChannelInteractionCode(channelInteraction.damageChannel1, channelInteraction.damageChannel2));
                channelInteractionCodes.Add(CalculateChannelInteractionCode(channelInteraction.damageChannel2, channelInteraction.damageChannel1));
            }
        }

        public static bool CanInteract(DamageAlignment damageAlignment1, DamageAlignment damageAlignment2, DamageChannel damageChannel1, DamageChannel damageChannel2)
        {
            return
                alignmentInteractionCodes.Contains(CalculateAlignmentInteractionCode(damageAlignment1, damageAlignment2)) &&
                channelInteractionCodes.Contains(CalculateChannelInteractionCode(damageChannel1, damageChannel2));
        }

        private static (DamageAlignment damageAlignment1, DamageAlignment damageAlignment2)[] GetAlignmentInteractions()
        {
            return new (DamageAlignment damageAlignment1, DamageAlignment damageAlignment2)[]
            {
                // Alignment Interactions Begin

                (DamageAlignment.Environment, DamageAlignment.Labyrinth),
                (DamageAlignment.Environment, DamageAlignment.Voyagers),
                (DamageAlignment.Environment, DamageAlignment.Neutrals),
                (DamageAlignment.Labyrinth, DamageAlignment.Voyagers),
                (DamageAlignment.Labyrinth, DamageAlignment.Neutrals),
                (DamageAlignment.Voyagers, DamageAlignment.Neutrals)

                // Alignment Interactions End
            };
        }

        private static (DamageChannel damageChannel1, DamageChannel damageChannel2)[] GetChannelInteractions()
        {
            return new (DamageChannel damageChannel1, DamageChannel damageChannel2)[]
            {
                // Channel Interactions Begin

                (DamageChannel.Props, DamageChannel.Props),
                (DamageChannel.Props, DamageChannel.Attacks),
                (DamageChannel.Attacks, DamageChannel.Entities),
                (DamageChannel.Entities, DamageChannel.Entities)

                // Channel Interactions End
            };
        }

        private static int CalculateAlignmentInteractionCode(DamageAlignment damageAlignment1, DamageAlignment damageAlignment2)
        {
            return (int)damageAlignment1 * (int)DamageAlignment.Count + (int)damageAlignment2;
        }

        private static int CalculateChannelInteractionCode(DamageChannel damageChannel1, DamageChannel damageChannel2)
        {
            return (int)damageChannel1 * (int)DamageAlignment.Count + (int)damageChannel2;
        }
    }
}
