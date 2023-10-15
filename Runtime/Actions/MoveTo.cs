using System;
using UnityEngine;

namespace BehaviourTreeBuilder
{
    [Serializable]
    [AddNodeMenu("Actions")]
    public class MoveTo : ActionNode
    {
        public enum MoveMethod
        {
            AddForce,
            AddImpulse,
            MoveTowards,
            Velocity,
            Steering
        }
        [SerializeField] private MoveMethod _moveMethod;
        [PropertyShowIf( "_usePosition", true)]
        [SerializeField] private NodeProperty<Transform> _target;
#if CORE_3D
        [SerializeField] private NodeProperty<Vector3> _position;
        private Vector3 _destination;
#else
        [SerializeField] private NodeProperty<Vector2> _position;
        private Vector2 _destination;
#endif
        [SerializeField] private NodeProperty<float>  _speed = new NodeProperty<float>(100);
        [SerializeField] private NodeProperty<float>  _rotateSpeed = new NodeProperty<float>(100);
        [SerializeField] private float  _successDistance = 1;
        [SerializeField] private float  _angleStopRotate = 5f;
        [SerializeField] private bool _usePosition;
        
        private Transform _transform;

        // OnStart is called immediately before execution. It is used to setup any variables that need to be reset from the previous run.
        protected override void OnStart()
        {
            _transform = context.Transform;
            _destination = _usePosition ? _position.Value :_target.Value.position;
        }

        // OnStop is called after execution on a success or failure.
        protected override void OnStop()
        {
        }

        protected override void OnFixedUpdate()
        {
            _destination = _usePosition ? _position.Value :_target.Value.position;
#if CORE_3D
            var moveDirection = _destination - _transform.position;
            moveDirection.Normalize();
            var velocity = moveDirection * (_speed.Value * Time.fixedDeltaTime);
            switch (_moveMethod)
            {
                case MoveMethod.AddForce:
                case MoveMethod.AddImpulse:
                    var forceMode = _moveMethod == MoveMethod.AddForce ? ForceMode.Force: ForceMode.Impulse; 
                    context.Rigidbody.AddForce(velocity, forceMode);
                    break;
                case MoveMethod.Velocity:
                    context.Rigidbody.velocity = velocity;
                    break;
                case MoveMethod.MoveTowards:
                    _transform.position = Vector3.MoveTowards(_transform.position, _position.Value,
                        _speed.Value * Time.fixedDeltaTime);
                    break;
                case MoveMethod.Steering:
                    context.Rigidbody.velocity = _transform.forward * (_speed.Value * Time.fixedDeltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_moveMethod), _moveMethod,"");
            }

            var forward = _transform.forward;
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            Quaternion rot = Quaternion.RotateTowards(_transform.rotation, targetRot, _rotateSpeed.Value * Time.fixedDeltaTime);
            if (Vector3.Angle(moveDirection, forward) >= _angleStopRotate)
            {
                context.Rigidbody.MoveRotation(rot);
            }
#else
            var moveDirection = _destination - (Vector2)_transform.position;
            moveDirection.Normalize();
            var velocity = moveDirection * (_speed.Value * Time.fixedDeltaTime);
            switch (_moveMethod)
            {
                case MoveMethod.AddForce:
                case MoveMethod.AddImpulse:
                    var forceMode = _moveMethod == MoveMethod.AddForce ? ForceMode2D.Force: ForceMode2D.Impulse; 
                    context.Rigidbody2D.AddForce(velocity, forceMode);
                    break;
                case MoveMethod.Velocity:
                    context.Rigidbody2D.velocity = velocity;
                    break;
                case MoveMethod.MoveTowards:
                    _transform.position = Vector2.MoveTowards(_transform.position, _position.Value,
                        _speed.Value * Time.fixedDeltaTime);
                    break;
                case MoveMethod.Steering:
                    context.Rigidbody2D.velocity = _transform.up * (_speed.Value * Time.fixedDeltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_moveMethod), _moveMethod,"");
            }

            var forward = _transform.up;
            Quaternion targetRot = Quaternion.LookRotation(forward, moveDirection);
            Quaternion rot = Quaternion.RotateTowards(_transform.rotation, targetRot, _rotateSpeed.Value * Time.fixedDeltaTime);
            if (Vector2.Angle(moveDirection, forward) >= _angleStopRotate)
            {
                context.Rigidbody2D.MoveRotation(rot);
            }
#endif
        }

        // OnUpdate runs the actual task.
        protected override State OnUpdate()
        {
#if CORE_3D
            var remainingDistance = Vector3.Distance(_destination, _transform.position);
#else
            var remainingDistance = Vector2.Distance(_destination, _transform.position);
#endif
            return remainingDistance <= _successDistance ? State.Success : State.Running;
        }

        public override string OnShowDescription()
        {
            if (_usePosition)
            {
                return $"Move to position {_position.Value}";
            }
            return $"Move to: {(_target.Value != null ? _target.Value.name : "null")}";
        }
    }
}
