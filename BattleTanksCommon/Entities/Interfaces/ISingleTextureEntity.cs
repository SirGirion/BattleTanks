namespace BattleTanksCommon.Entities.Interfaces
{
    /// <summary>
    /// Interface for any entity that only needs to draw one texture.
    /// </summary>
    public interface ISingleTextureEntity
    {
        /// <summary>
        /// Name of the texture to load from the atlas.
        /// </summary>
        string TextureName { get; set; }
    }
}
