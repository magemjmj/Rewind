using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode
{
    public enum Status
    {
        Invalid,
        None,
        Success,
        Failure,
        Running,
    };

    public enum Aborts
    {
        None,
        Self,
        LowerPriority,
        Both,
    };

	protected Status m_status = Status.Invalid;
    protected Status m_decostatus = Status.Invalid;
    protected Aborts m_observeraborts = Aborts.None;
    protected BlackBoard m_blackboard = null;
    protected List<BTNode> m_decorators;

    public void AddDecorator(BTNode child) { m_decorators.Add(child); }

    public virtual Status Update(float fDelta) { return Status.Invalid; }
    public virtual void Initialize() { }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public virtual Status OnTick(float fDelta)
    {
        foreach (var decorator in m_decorators)
        {
            var status = decorator.OnTick(fDelta);
            if (status != Status.Success)
            {
                if (m_status == Status.Running)
                {
                    if (m_observeraborts == Aborts.None)
                    {
                        continue;
                    }
                    else

                    if (m_observeraborts == Aborts.Self)
                    {
                        return Status.Failure;
                    }
                    else

                    if (m_observeraborts == Aborts.LowerPriority)
                    {
                        return Status.Failure;
                    }
                    else

                    if (m_observeraborts == Aborts.Both)
                    {
                        return Status.Failure;
                    }
                }
                else
                {
                    return Status.Failure;
                }
            }
        }

        if (m_status == Status.Invalid)
        {
            Initialize();
        }

        if (m_status != Status.Running)
        {
            OnEnter();
        }

        m_status = Update(fDelta);

        if (m_status != Status.Running)
        {
            OnExit();
        }

        return m_status;
    }

    public void SetObserverAborts(Aborts aborts) { m_observeraborts = aborts; }

    public bool IsSuccess() { return m_status == Status.Success; }
    public bool IsFailure() { return m_status == Status.Failure; }
    public bool IsRunning() { return m_status == Status.Running; }
    public bool IsExited() { return IsSuccess() || IsFailure(); }
    public void Reset() { m_status = Status.Invalid; }

    public void SetBlackBoard(BlackBoard board) { m_blackboard = board; }
    public BlackBoard GetBlackBoard() { return m_blackboard; }

    public virtual void SetSharedBlackBoard(BlackBoard board)
    {
        m_blackboard = board;

        foreach (var bb in m_decorators)
        {
            bb.SetSharedBlackBoard(board);
        }
    }

    public virtual void AddChild(BTNode node) { }
}

public class BTService : BTNode
{
    protected float m_fInterval = 0.0f;
    protected float m_fAccumTime = 0.0f;

    public BTService() {}
    public BTService(float fInterval) { m_fInterval = fInterval; }

    public void SetInverval(float fInterval) { m_fInterval = fInterval; }

    public override Status OnTick(float fDelta)
	{
		while (m_fAccumTime >= m_fInterval)
		{
			m_fAccumTime -= m_fInterval;
		}

		if (m_status == Status.Invalid)
		{
            Initialize();
		}

		if (m_status != Status.Running)
		{
            OnEnter();
            m_status = Status.Running;
		}

		m_fAccumTime += fDelta;
		if (m_fAccumTime >= m_fInterval)
		{
			m_status = Update(m_fAccumTime);
            m_fAccumTime = 0.0f;
		}

		if (m_status != Status.Running)
		{
            OnExit();
		}

		return m_status;
	}

};

public class BTComposite : BTNode
{
	protected List<BTNode> m_children;
    protected int m_index = 0;

    protected List<BTNode> m_services;

    public override void AddChild(BTNode child) { m_children.Add(child); }
    public bool HasChild() { return m_children.Count > 0; }
    public int GetIndex() { return m_index; }

    public void AddService(BTNode service) { m_services.Add(service); }

    public override void SetSharedBlackBoard(BlackBoard board)
	{
		base.SetSharedBlackBoard(board);

		foreach (var bb in m_children)
		{
			bb.SetSharedBlackBoard(board);
		}

        foreach (var bb in m_services)
		{
			bb.SetSharedBlackBoard(board);
		}
	}
};

public class BTSequence : BTComposite
{
	public override void Initialize()
	{
		m_index = 0;
	}

    public override Status Update(float fDelta)
	{
		foreach (var service in m_services)
		{
			service.OnTick(fDelta);
		}

		if (!HasChild())
		{
			return Status.Success;
		}

		while (true)
		{
			var child = m_children[m_index];
			var status = child.OnTick(fDelta);

			if (status == Status.Failure)
			{
				m_index = 0;
				return status;
			}

			if (status == Status.Running)
			{
				return status;
			}

			if (++m_index == m_children.Count)
			{
				m_index = 0;
				return status;
			}
		}
	}
};

public class BTSelector : BTComposite
{
    public override void Initialize()
	{
		m_index = 0;
	}

    public override Status Update(float fDelta)
	{
		if (!HasChild())
		{
			return Status.Success;
		}

		while (true)
		{
			var child = m_children[m_index];
			var status = child.OnTick(fDelta);

			if (status == Status.Success)
			{
				m_index = 0;
				return status;
			}

			if (status == Status.Running)
			{
				return status;
			}

			if (++m_index == m_children.Count)
			{
				m_index = 0;
				return status;
			}
		}
	}
};

public abstract class BTAction : BTNode
{
    public BTAction() {}
    public abstract Status Update(float fDelta);

};

public class BTWaitAction : BTAction
{
	protected float m_fWaitTime = 0.0f;
    protected float m_fAccumTime = 0.0f;

    public BTWaitAction() {}
    public BTWaitAction(float fWaitTime) { m_fWaitTime = fWaitTime; }

    public override void OnEnter()
	{
		m_fAccumTime = 0.0f;
	}

    public override Status Update(float fDelta)
	{
		m_fAccumTime += fDelta;

		if (m_fAccumTime<m_fWaitTime)
		{
			return Status.Running;
		}
		else
		{
			return Status.Success;
		}
	}

    public void SetWaitTime(float fWaitTime) { m_fWaitTime = fWaitTime; }

};

public class BTRandomAction : BTAction
{
    protected float m_fPercent = 0.0f;

    public BTRandomAction()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
    }


    public BTRandomAction(float fPercent /* 0.0 ~ 1.0 */)
    {
        m_fPercent = fPercent;
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public override Status Update(float fDelta)
	{
        float fran = Random.Range(0.0f, 1.0f);

		if (fran <= m_fPercent)
		{
			return Status.Success;
		}
		else
		{
			return Status.Failure;
		}
	}

	void SetPercent(float fPercent) { m_fPercent = fPercent; }

};

public class BTBehaviorTree : BTNode
{
    private BTNode m_child = new BTNode();

    public BTBehaviorTree() {}

    public override Status Update(float fDelta)
    {
        return m_child.OnTick(fDelta);
    }

    public override void SetSharedBlackBoard(BlackBoard board)
	{
		base.SetSharedBlackBoard(board);
		m_child.SetSharedBlackBoard(board);
	}

    public override void AddChild(BTNode node) { m_child = node; }

	public BTNode GetChild() { return m_child; }

};
