using System;
using System.Windows;

namespace MdiMvvm.Interfaces
{
    public interface IMdiWindowViewModel
    {
        IMdiContainerViewModel Container { get; set; }

        /// <summary>
        /// GUID of window
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Title
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Is window modal
        /// </summary>
        bool IsModal { get; set; }

        /// <summary>
        /// Is window selected at <see cref="MdiContainer"/>
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Previous position's left
        /// </summary>
        double PreviousLeft { get; set; }

        /// <summary>
        /// Previous position's top
        /// </summary>
        double PreviousTop { get; set; }

        /// <summary>
        /// Previous position's width
        /// </summary>
        double PreviousWidth { get; set; }

        /// <summary>
        /// Previous position's height
        /// </summary>
        double PreviousHeight { get; set; }

        /// <summary>
        /// Previous position's window state
        /// </summary>
        WindowState PreviousState { get; set; }

        /// <summary>
        /// Current position's left
        /// </summary>
        double CurrentLeft { get; set; }

        /// <summary>
        /// Current position's top
        /// </summary>
        double CurrentTop { get; set; }

        /// <summary>
        /// Current position's width
        /// </summary>
        double CurrentWidth { get; set; }

        /// <summary>
        /// Current position's height
        /// </summary>
        double CurrentHeight { get; set; }

        /// <summary>
        /// Current position's window state
        /// </summary>
        WindowState WindowState { get; set; }

    }
}
