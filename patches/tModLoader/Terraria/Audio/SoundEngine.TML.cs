using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Utilities;
using Terraria.ID;
using Terraria.ModLoader;

#nullable enable

namespace Terraria.Audio
{
	partial class SoundEngine
	{
		// Public API methods

		public static SlotId PlaySound(in SoundStyle style, Vector2? position = null) {
			if (Main.dedServ || !IsAudioSupported) {
				return SlotId.Invalid;
			}

			return SoundPlayer.Play(in style, position);
		}

		/// <inheritdoc cref="SoundPlayer.TryGetSound(SlotId, out ActiveSound?)"/>
		public static bool TryGetSound(SlotId slotId, out ActiveSound? result) {
			if (Main.dedServ || !IsAudioSupported) {
				result = null;
				return false;
			}

			return SoundPlayer.TryGetSound(slotId, out result);
		}

		// Internal redirects

		internal static SoundEffectInstance? PlaySound(SoundStyle? style, Vector2? position = null) {
			if (style == null)
				return null;
			
			var slotId = PlaySound(style.Value, position);

			return slotId.IsValid ? GetActiveSound(slotId)?.Sound : null;
		}

		internal static SoundEffectInstance? PlaySound(SoundStyle? type, int x, int y) //(SoundStyle type, int x = -1, int y = -1)
			=> PlaySound(type, XYToOptionalPosition(x, y));

		internal static void PlaySound(int type, Vector2 position, int style = 1)
			=> PlaySound(type, (int)position.X, (int)position.Y, style);

		internal static SoundEffectInstance? PlaySound(int type, int x = -1, int y = -1, int Style = 1, float volumeScale = 1f, float pitchOffset = 0f) {
			if (!SoundID.TryGetLegacyStyle(type, Style, out var soundStyle)) {
				Logging.tML.Warn($"Failed to get legacy sound style for ({type}, {Style}) input.");

				return null;
			}

			soundStyle = soundStyle with {
				Volume = soundStyle.Volume * volumeScale,
				Pitch = soundStyle.Pitch + pitchOffset
			};

			var slotId = PlaySound(soundStyle, XYToOptionalPosition(x, y));

			return slotId.IsValid ? GetActiveSound(slotId)?.Sound : null;
		}

		internal static SlotId PlayTrackedSound(in SoundStyle style, Vector2? position = null)
			=> PlaySound(in style, position);

		internal static ActiveSound? GetActiveSound(SlotId slotId)
			=> TryGetSound(slotId, out var result) ? result : null;

		// Utilities

		private static Vector2? XYToOptionalPosition(int x, int y)
			=> x != -1 || y != -1 ? new Vector2(x, y) : null;
	}
}
