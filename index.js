function patch_projectile(record, settingobj) {
	if (settingobj.enabled) {
		xelib.SetFloatValue(record, 'DATA\\Speed', settingobj.speed);
		xelib.SetFloatValue(record, 'DATA\\Gravity', settingobj.gravity);
		xelib.SetFloatValue(record, 'DATA\\Impact Force', settingobj.impactforce);
		xelib.SetValue(record, 'VNAM', settingobj.noiselevel)
    }
}

// Another Archery Tweak zEdit Patcher by radj307
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
			disable_dodge: false,
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
			helpers.logMessage('BEGIN');
		},

		process: [
			{ // Game Settings
				load: {
					signature: 'GMST',
					filter: rec => true
				},
				patch: rec => {
					let editorID = xelib.GetValue(rec, 'EDID');
					if (settings.disable_dodge) { // TODO: Implement a checkbox for this in settings.html
						if (editorID.includes('fCombatDodgeChanceMax')) {
							xelib.SetValue(rec, 'DATA\\Float', '0.000000');
						}
					}
					if (settings.disable_autoaim) {
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
					if (xelib.GetValue(rec, 'DATA\\Type').includes('Arrow')) { // Projectile is an arrow/bolt type.
						// Disable supersonic flag if settings allow
						if (settings.disable_supersonic) {
							let supersonic = xelib.GetFlag(rec, 'DATA\\Flags', 'Supersonic');			// Get supersonic flag state
							if (supersonic) { xelib.SetFlag(rec, 'DATA\\Flags', 'Supersonic', false); }	// If true then remove supersonic flag
						}
						// Modify stats for arrows / bolts
						if (settings.arrow.enabled && (editorID.includes('Arrow') || editorID == "DLC1AurielsBloodDippedProjectile")) { // Projectile is an arrow
							helpers.logMessage('[ARROW]: ' + editorID); // debug
							patch_projectile(rec, settings.arrow);
							if (settings.arrow.gravity > 0 && editorID == "DLC1AurielsBloodDippedProjectile" || editorID == "DCL1ArrowElvenBloodProjectile") {
								helpers.logMessage('Removing gravity from ' + editorID)
								xelib.SetFloatValue(rec, 'DATA\\Gravity', 0.000000);
							}
						}
						else if (settings.bolt.enabled && editorID.includes('Bolt')) { // Projectile is a bolt
							helpers.logMessage('[BOLT]: ' + editorID); // debug
							patch_projectile(rec, settings.bolt);
						}
						else {
							helpers.logMessage('Skipped projectile: ' + editorID);
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