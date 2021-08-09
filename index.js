registerPatcher({
	info: info,								// ? Object Type ?
	gameModes: [xelib.gmTES5, xelib.gmSSE],	// Determines which games this module applies to

	settings: {								// Settings Object, for receiving user-settings from HTML config
		label: 'aatpatcher',
		templateUrl: `${patcherUrl}/partials/settings.html`,	// Path to the HTML config file
		defaultSettings: {					// Defines default variables
			patchFileName: 'Another_Archery_Tweak.esp',
			disable_autoaim: true,
			disable_supersonic: true,
			arrow: {
				enabled: true,
				speed: 5000.000000,
				gravity: 0.340000,
				impactforce: 0.440000,
				noiselevel: "Silent",
			},
			bolt: {
				enabled: true,
				speed: 5800.000000,
				gravity: 0.340000,
				impactforce: 0.440000,
				noiselevel: "Normal",
			},
		},
	},

	requiredFiles: [],
	getFilesToPatch: function(filenames) {
		return filenames;
	},

	execute: (patchFile, helpers, settings, locals) => ({
		initialize: function() {
			helpers.logMessage(' ------------------------------------------ ');
			helpers.logMessage('    Another Archery Tweak');
			helpers.logMessage('             Version 1.0');
			helpers.logMessage(' ------------------------------------------ ');
			helpers.logMessage('Configuration:');
			helpers.logMessage(settings.aatpatcher);
			if (settings.arrow.enabled) {
				helpers.logMessage('[ARROWS]');
				helpers.logMessage('arrow.speed       ' + settings.arrow.speed);
				helpers.logMessage('arrow.gravity     ' + settings.arrow.gravity);
				helpers.logMessage('arrow.impactforce ' + settings.arrow.impactforce);
				helpers.logMessage('arrow.noiselevel  ' + settings.arrow.noiselevel);
			}
			if (settings.bolt.enabled) {
				helpers.logMessage('[BOLTS]');
				helpers.logMessage('bolt.speed       ' + settings.bolt.speed);
				helpers.logMessage('bolt.gravity     ' + settings.bolt.gravity);
				helpers.logMessage('bolt.impactforce ' + settings.bolt.impactforce);
				helpers.logMessage('bolt.noiselevel  ' + settings.bolt.noiselevel);
            }
			helpers.logMessage('BEGIN');
		},

		process: [
			{ // Game Settings
				load: {
					signature: 'GMST',
					filter: rec => true
				},
				patch: rec => {
					if (settings.disableautoaim) {
						let editorID = xelib.GetValue(rec, 'EDID');
						if (editorID.includes('fAutoAimMaxDegrees')) {
							xelib.SetValue(rec, 'DATA\\Float', '0.000000');
							helpers.logMessage('Set fAutoAimMaxDegrees to 0.');
						}
						else if (editorID.includes('fAutoAimMaxDegrees3rdPerson')) {
							xelib.SetValue(rec, 'DATA\\Float', '0.000000');
							helpers.logMessage('Set fAutoAimMaxDegrees3rdPerson to 0.');
						}
						else if (editorID.includes('fAutoAimMaxDistance')) {
							xelib.SetValue(rec, 'DATA\\Float', '0.000000');
							helpers.logMessage('Set fAutoAimMaxDistance to 0.');
						}
						else if (editorID.includes('fAutoAimScreenPercentage')) {
							xelib.SetValue(rec, 'DATA\\Float', '0.000000');
							helpers.logMessage('Set fAutoAimScreenPercentage to 0.');
						}
                    }
				}
			}, // End Game Settings

			{ // Projectiles
				load: {
					signature: 'PROJ',
					filter: rec => true
				},
				patch: rec => {
					let editorID = xelib.GetValue(rec, 'EDID');
					let proj_t = xelib.GetValue(rec, 'DATA\\Type');
					if (proj_t.includes('Arrow')) { // Projectile is an arrow/bolt type.
						if (editorID.includes('Arrow') && settings.arrow.enabled) { // Projectile is an arrow
							helpers.logMessage('[ARROW]: ' + editorID); // debug
							xelib.SetFloatValue(rec, 'DATA\\Speed', settings.arrow.speed);				// Apply arrow speed
							xelib.SetFloatValue(rec, 'DATA\\Gravity', settings.arrow.gravity);			// Apply arrow gravity
							xelib.SetFloatValue(rec, 'DATA\\Impact Force', settings.arrow.impactforce);	// Apply arrow impact force
							xelib.SetValue(rec, 'VNAM', settings.arrow.noiselevel);						// Apply arrow noise level
							let supersonic = xelib.GetFlag(rec, 'DATA\\Flags', 'Supersonic');			// Get supersonic flag state
							if (supersonic) { xelib.SetFlag(rec, 'DATA\\Flags', 'Supersonic', false); }	// If true then remove supersonic flag
						}
						else if (editorID.includes('Bolt') && settings.bolt.enabled) { // Projectile is a bolt
							helpers.logMessage('[BOLT]: ' + editorID); // debug
							xelib.SetFloatValue(rec, 'DATA\\Speed', settings.bolt.speed);				// Apply bolt speed
							xelib.SetFloatValue(rec, 'DATA\\Gravity', settings.bolt.gravity);			// Apply bolt gravity
							xelib.SetFloatValue(rec, 'DATA\\Impact Force', settings.bolt.impactforce);	// Apply bolt impact force
							xelib.SetValue(rec, 'VNAM', settings.bolt.noiselevel);						// Apply bolt noise level
							let supersonic = xelib.GetFlag(rec, 'DATA\\Flags', 'Supersonic');			// Get supersonic flag state
							if (supersonic) { xelib.SetFlag(rec, 'DATA\\Flags', 'Supersonic', false); }	// If true then remove supersonic flag
						}
						else {
							helpers.logMessage('Unrecognized Projectile Type (May require adding to overrides list): ' + editorID);
						} // debug
					}
				}
			}, // End Projectiles
		],

		finalize: function() {
			helpers.logMessage('END');
			helpers.logMessage(' ------------------------------------------ ');
			helpers.logMessage('    Another Archery Tweak');
			helpers.logMessage('             Version 1.0');
			helpers.logMessage(' ------------------------------------------ ');
		}
	})
});