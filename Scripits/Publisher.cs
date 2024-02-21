using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Publisher : NoneMonoinstance<Publisher>
{
    public event EventHandler EmyEvent;
    private event EventHandler EmyEventNonePar;
    protected virtual void OnMyEvent(EventArgs e)
    {
        EmyEvent?.Invoke(this, e);
    }
    public void TriggerEvent(EventArgs e)
    {
        OnMyEvent(e);

    }
    //protected virtual void OnMyEvent()
    //{

    //    EmyEventNonePar?.Invoke(this,EventArgs.Empty);
    
    //}
    //public EventHandler GetPublisherEventNonePar()
    //{

    //    return EmyEventNonePar;
    //}
    public EventHandler GetPublisherEvent()
    {

        return EmyEvent;
    }

    void EventTest()//i am subscriber and trriger happen in here and in camera
    {

        //Subscriber subscriber = new Subscriber();
        //subscriber.Subscribe(Publisher.GetInstance());

        //Publisher.GetInstance().TriggerEvent(EventArgs.Empty);

    }
   public void DisposeEvent()
    { 
    EmyEvent = null;
    
    }
}

public class Subscriber
{
    public void Subscribe()
    {
        var handel = Publisher.Getinstance().GetPublisherEvent();

        handel += OnMyEventHappen;
    }

    private void OnMyEventHappen(object sender, EventArgs e)
    {
        Debug.Log("Event has been triggered.");
    }
    private void OnMyEventHappenNonePar()
    {
        Debug.Log("Event has been triggered.");
    }

    public class MyCustomEventArgs : EventArgs//the custom event args base of eventargs.empty
    {
        public string Message { get; set; }

        public MyCustomEventArgs(string message)
        {
            Message = message;
        }
    }

}
