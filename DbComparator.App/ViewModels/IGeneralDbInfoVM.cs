using System;

namespace DbComparator.App.ViewModels
{
    public interface IGeneralDbInfoVM
    {
        /// <summary>
        /// Handler for the message delivery
        /// </summary>
        event EventHandler MessageHandler;

        /// <summary>
        /// Message delivery handler with the current entity
        /// </summary>
        event EventHandler CurrentEntitiesMessageHandler;

        /// <summary>
        /// The cleaning process
        /// </summary>
        /// <param name="depth">Depth Of Cleaning</param>
        void ClearInfoObjects(DepthOfCleaning depth);
    }
}
