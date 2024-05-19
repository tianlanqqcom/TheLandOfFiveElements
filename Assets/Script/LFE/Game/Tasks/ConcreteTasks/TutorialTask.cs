/*
 * File: TutorialTask.cs
 * Description: 新手教程，taskID=1001
 * Author: tianlan
 * Last update at 24/5/17   14:50
 *
 * Update Records:
 * tianlan  24/5/17
 */

using Script.GameFramework.Core;
using UnityEngine;
using Script.GameFramework.Game.Tasks.TaskNodes;
using Script.GameFramework.Game.Tasks;
using Script.GameFramework.GamePlay;
using Script.GameFramework.GamePlay.InteractiveSystem;
using Script.LFE.Game.Tasks.TaskNodes;
using UnityEngine.Assertions;

namespace Script.LFE.Game.Tasks.ConcreteTasks
{
    public class TutorialTask : Task
    {
        private const int TutorialTaskID = 1001;

        #region TaskNodeTriggers

        private GameObject _beginTutorialTask;
        private GameObject _turnWithMouseMove;
        private GameObject _jump;
        private GameObject _restartPoint;
        private GameObject _restartWhenFallInWater;
        private GameObject _finishJumpAndSeeStone;
        private GameObject _stone;
        private GameObject _beginFightTutorial;
        private GameObject _endTutorial;

        #endregion

        public TutorialTask()
        {
            TaskID = TutorialTaskID;
            Name = new FixedString("TaskNames", "tutorialName");

            FindNodeObjects();

            GotoTargetPositionTaskNode move = new GotoTargetPositionTaskNode(
                new FixedString("TaskMessages", "tutorial_move_desp"),
                new FixedString("TaskMessages", "tutorial_move_desp"),
                TutorialTaskID,
                0,
                _turnWithMouseMove.transform.position,
                _turnWithMouseMove.transform.localScale
            );
            SetBeginNode(move);

            GotoTargetPositionTaskNode turn = new GotoTargetPositionTaskNode(
                new FixedString("TaskMessages", "tutorial_turn_desp"),
                new FixedString("TaskMessages", "tutorial_turn_desp"),
                TutorialTaskID,
                1,
                _jump.transform.position,
                _jump.transform.localScale
            );
            move.SetNext(turn);

            GotoTargetPositionTaskNode jump = new GotoTargetPositionTaskNode(
                new FixedString("TaskMessages", "tutorial_jump_desp"),
                new FixedString("TaskMessages", "tutorial_jump_desp"),
                TutorialTaskID,
                2,
                _finishJumpAndSeeStone.transform.position,
                _finishJumpAndSeeStone.transform.localScale
            );
            turn.SetNext(jump);

            var interactiveItem = _restartWhenFallInWater.AddComponent<InteractiveItem>();
            interactiveItem.IsAutoPlay = true;
            interactiveItem.PlayOnce = false;
            interactiveItem.InteractiveAction.AddListener(() =>
            {
                MyGameMode.Instance.RestartPlayerAtWithRotation(
                    _restartPoint.transform.position,
                    _restartPoint.transform.rotation,
                    1f);
            });

            DialogTaskNode dialogWithStone = new DialogTaskNode(
                new FixedString("TaskMessages", "tutorial_stone_desp"),
                new FixedString("TaskMessages", "tutorial_stone_desp"),
                TutorialTaskID,
                3,
                _stone.transform.position,
                "TutorialDialogStone"
            );
            jump.SetNext(dialogWithStone);

            GotoTargetPositionTaskNode readyToBeginFight = new GotoTargetPositionTaskNode(
                new FixedString("TaskMessages", "tutorial_begin_fight_desp"),
                new FixedString("TaskMessages", "tutorial_begin_fight_desp"),
                TutorialTaskID,
                4,
                _beginFightTutorial.transform.position
            );
            
            dialogWithStone.SetNext(readyToBeginFight);
        }

        private void FindNodeObjects()
        {
            const string path = "TaskTriggerNodes/";
            _beginTutorialTask = GameObject.Find(path + "BeginTutorialTask");
            _turnWithMouseMove = GameObject.Find(path + "TurnWithMouseMove");
            _jump = GameObject.Find(path + "JumpTutorial");
            _restartPoint = GameObject.Find("WorldStatic/RestartPoint");
            _restartWhenFallInWater = GameObject.Find(path + "RestartPlayerWhenFallInWater");
            _finishJumpAndSeeStone = GameObject.Find(path + "FinishJump_SeeStone");
            _stone = GameObject.Find(path + "Stone");
            _beginFightTutorial = GameObject.Find(path + "BeginFightTutorial");
            _endTutorial = GameObject.Find(path + "EndTutorial");

            Assert.IsTrue(_beginTutorialTask);
            Assert.IsTrue(_turnWithMouseMove);
            Assert.IsTrue(_jump);
            Assert.IsTrue(_restartPoint);
            Assert.IsTrue(_restartWhenFallInWater);
            Assert.IsTrue(_finishJumpAndSeeStone);
            Assert.IsTrue(_stone);
            Assert.IsTrue(_beginFightTutorial);
            Assert.IsTrue(_endTutorial);
        }
    }
}