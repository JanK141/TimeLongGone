# Programing

## Player controller
With the aim of controller being dynamic and not making player feel like his actions are in any way restricted in this fast paced environment of constant battle, we designed his moveset to include fast transitions from any state with ability to interrupt crucial animations with any defensive move.

![Player - State Machine only](https://user-images.githubusercontent.com/36763441/225680868-8127e701-1c42-4656-8bb3-f497ed22cd1b.png)

Which then we implemented by creating following classes:
![Player - States](https://user-images.githubusercontent.com/36763441/225685398-8bf90adc-7c3b-407a-8b6b-66187302fca5.png)


## Rewinding time
Being able to control time is a very crucial mechanic in our game, as our standard health bar is replaced with a need of rewinding time every time player gets hit. On top of that player is able to slow down time at any point to make some aspects of a fight easier to him. Managing all of this ended up not complicated, yet it required a lot of thought due to its high susceptibility to errors in logic.

![Time Control](https://user-images.githubusercontent.com/36763441/225689882-e4527cc5-4e92-4e3f-8225-14d5c446b975.png)

To properly rewind time every game object that is rewindable cosists of components responsible for recording and re-applying single aspect of objects state:

![Time Components](https://user-images.githubusercontent.com/36763441/225691964-b9ea79a7-b987-4b39-a63e-be22291aa831.PNG)


## Enemy AI
To implement logic behinde our enemies behavior we decided to use state machines, but we needed separate instances of them to manage every phase of a fight on every difficulty level differently. To avoid biolerplating we created basic prototype of a dedicated tool.

![OldFSMC](https://user-images.githubusercontent.com/36763441/225695780-1fb28b70-d72e-44fb-9f55-231201f1b29f.PNG)

Currently newer version is being developed with more visual approach using Unity's new UI Toolkit and component-based states creation process, strongly inspired by mecanim.

![FSMC](https://user-images.githubusercontent.com/36763441/225696658-0a23c1b0-6b78-47d2-8ea9-3349820449cb.PNG)
