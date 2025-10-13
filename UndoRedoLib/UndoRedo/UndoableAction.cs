// Copyright (c) 2025 Sarah Frémann.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace UndoRedoLib.UndoRedo
{
    /// <summary>
    /// Base struct for undoable action
    /// </summary>
    /// <param name="DoDelegate"></param>
    /// <param name="UndoDelegate"></param>
    public struct UndoableAction(Action DoDelegate, Action UndoDelegate)
    {
        public Action Do { get; } = DoDelegate;
        public Action Undo { get; } = UndoDelegate;
        public string Description { get; set; } = "";
    }
}
