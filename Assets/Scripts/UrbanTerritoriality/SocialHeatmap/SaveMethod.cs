namespace UrbanTerritoriality.Enum
{
	/** The saving method of the visibility map */
	public enum SaveMethod
	{
		/** The map will be saved after some specified time. */
		TIME,
		/** The map will be saved when the quality of it reaches a certian level. */
		QUALITY,
		/** The map will be saved when we stop the editor from running */
		SIMULATION_END,
	}
}