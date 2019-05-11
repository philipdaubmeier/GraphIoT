namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    /// <summary>
    /// Represents the method that will handle the event raised when a scene is changed in the twin model.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A SceneChangedEventArgs that contains the event data,
    /// i.e. the scene and in which zone and group the change happened.</param>
    public delegate void SceneChangedEventHandler(object sender, SceneChangedEventArgs e);
}