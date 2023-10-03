//setup
var MapSize = 256;
var Map = [""];
var ThingX = [,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,];
var ThingY = [,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,];
var ThingTraits = ["","","","","","","","","","","","","","", "", "", "", "", "", "", "","","","","","","","","","","","","","","","","","","",""];
var ThingChar = [];
var PriorPos = [,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,];
var DebugClick = 0;
var Seed = 1;
var ObjectCount = 0;

var Player = SpawnThing("꘨");
var Cat = SpawnThing("c");

function GameLoop() {
  DebugClick = DebugClick + 1;
  var MapPrint ="";
  // Margin scripts, for, well margins without it, all the lines would be squished together
  var Margin = "";
  for (var i = 0; i < 29 -Math.sqrt(MapSize); i++) {
    Margin += " ";
  }
  for (var i = 0; i < MapSize; i++) {
    //stops startup lag
    if (DebugClick > 1) {
      removeItem(Map, i);
    }
    
    insertItem(Map, i, "`");
  }
  // code body
  
  
  SpawnTrees();
  Animals();
  Traits();

  
  //print objects onto map
  for (var i = 0; i < ObjectCount; i++) {
    Thing(i);
  }
  //print Map
  for (var i = 0; i < Math.sqrt(MapSize); i++) {
    MapLine = "";
    for (var i2 = 0; i2 < Math.sqrt(MapSize); i2++) {
      var MapLine = MapLine + Map[(i2 + i * Math.sqrt(MapSize))];
    }
    // to directly put on screen write(MapLine);
    MapPrint += MapLine + Margin;
    setText("text_area2",MapPrint);
    }


};

function Thing(Id) {
//setup
var X = ThingX[Id];
var Y = ThingY[Id];
  //Stops Null Erorrs
if(X == null){
    X=0;
  }
if(Y == null){
    Y=0;
  }
  
  //Finish
  var Pos = X + Y * Math.sqrt(MapSize);
  removeItem(Map, Pos);
  insertItem(Map, Pos, ThingChar[Id]);
  removeItem(ThingX, Id);
  insertItem(ThingX, Id, X);
  removeItem(ThingY, Id);
  insertItem(ThingY, Id, Y);
  //stops moving offscreen
NoClip(Id);
//encodes prior position to be used in things like colition scripts
removeItem(PriorPos, Id);
insertItem(PriorPos, Id, EncodeDouble(X, Y));
}
function SpawnTrees() {
  for (var i = 0; i < MapSize; i++) {
    if (Math.sqrt(i * Seed + 4756) / Math.round(Math.sqrt(i * Seed + 4756)) == 1) {
      removeItem(Map, i);
      insertItem(Map, i, "t");
    }
  }
}
//find Id for thing
function SpawnThing(char) {
  appendItem(ThingChar, char);
  insertItem(ThingX, ThingChar.length -1, 1);
  insertItem(ThingY, ThingChar.length -1, 1);
  ObjectCount++;
  return (ThingChar.length-1);
}
//move Scripts
function MoveUp(Id) {
  var y = ThingY[Id];
  removeItem(ThingY, Id);
  insertItem(ThingY, Id, y - 1);

}
function MoveDown(Id) {
  var y = ThingY[Id];
  removeItem(ThingY, Id);
  insertItem(ThingY, Id, y + 1);
}
function MoveLeft(Id) {
  var x = ThingX[Id];
  removeItem(ThingX, Id);
  insertItem(ThingX, Id, x - 1);
}
function MoveRight(Id) {
  var x = ThingX[Id];
  removeItem(ThingX, Id);
  insertItem(ThingX, Id, x + 1);
}
//End of move srctpis
onEvent("Startup", "click", function( ) {
  //spawn Squirls
  for (var i = 0; i < MapSize; i++) {
    if (Math.sqrt(i * Seed + 4756) / Math.round(Math.sqrt(i * Seed + 4756)) == 1) {
      var SetupSquirl = SpawnThing("s");
      SetX(SetupSquirl,i);
      SetY(SetupSquirl,i);
    }
  }
  SpawnRiver(Math.round((Math.round(Math.pow(10, Seed) * Math.sqrt(2)) - (Math.round(Math.pow(10, Seed-1) * Math.sqrt(2)))*10)*(Math.sqrt(MapSize) / 10)));
});
function DestroyThing(Id) {
  removeItem(ThingChar, Id);
  removeItem(ThingX, Id);
  removeItem(ThingY, Id);
  ObjectCount = ObjectCount - 1;
}
function SetX(Id,X) {
  removeItem(ThingX, Id);
  insertItem(ThingX, Id, X);
}
function SetY(Id,Y) {
  removeItem(ThingY, Id);
  insertItem(ThingY, Id, Y);
}
function raycast(Id,VectorX,VectorY) {
  var ray = [,,,,,,,,,,,,,,,,,,,,,,,,,];
  var RayX = ThingX[Id];
  var RayY = ThingY[Id];
  var Raypos = 0;
  while ((Map[(RayY * Math.sqrt(MapSize) + RayX)] == "`")) {
    Raypos += 1;
    RayX += VectorX;
    RayY += VectorY;
  }
  return Map[(RayY * Math.sqrt(MapSize) + RayX)];
  
}
onEvent("Up", "click", function( ) {
  MoveUp(Player);
});
onEvent("Down", "click", function( ) {
  MoveDown(Player);
});
onEvent("Left", "click", function( ) {
  MoveLeft(Player);
});
onEvent("Right", "click", function( ) {
  MoveRight(Player);
});
function Animals() {
  //repeat forloop for each kind of animal
  for (var i = 0; i < ThingChar.length; i++) {
    if (ThingChar[i] == "s") {
      SquirlAi(i);
    }
  }
  
    for (var i = 0; i < ThingChar.length; i++) {
    if (ThingChar[i] == "c") {
      CatAi(i);
    }
  }
}
function Traits() {
  for (var Id = 0; Id < ThingTraits.length; Id++) {
    if (ThingTraits[Id].includes("Rodentivore")) {
    Rodentivore(Id);
    }
    if (ThingTraits[Id].includes("Immobile")) {
    Immobile(Id);
    }
    
  }
}
function SquirlAi(Id) {
  var Memory = [];
  var State = "Wander";
  appendItem(Memory, raycast(Id, 1, 0));
  appendItem(Memory, raycast(Id, -1, 0));
  appendItem(Memory, raycast(Id, 0, 1));
  appendItem(Memory, raycast(Id, 0, -1));
  appendItem(Memory, raycast(Id, 1, 1));
  appendItem(Memory, raycast(Id, 1, -1));
  appendItem(Memory, raycast(Id, -1, 1));
  appendItem(Memory, raycast(Id, -1, -1));
  for (var i = 0; i < Memory.length; i++) {
    if (Memory[i] == "c") {
      State = "Flee";
    }
  }
  if (State == "Flee") {
    for (var i = 0; i < Memory.length; i++) {
      if (Memory[i] == "t") {
        if (i == 1) {
          MoveRight(Id);
        } else {
          if (i == 2) {
            MoveLeft(Id);
          } else {
            if (i == 3) {
              MoveDown(Id);
            } else {
              if (i == 4) {
                MoveUp(Id);
              } else {
                if (i == 5) {
                  MoveDown(Id);
                  MoveRight(Id);
                } else {
                  if (i == 6) {
                    MoveUp(Id);
                    MoveRight(Id);
                  } else {
                    if (i == 7) {
                      MoveUp(Id);
                      MoveLeft(Id);
                    } else {
                      MoveDown(Id);
                      MoveLeft(Id);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  }
  if (State == "Wander") {
    var x = randomNumber(1, 4);
    if (x == 1) {
      MoveUp(Id);
    } else {
      if (x == 2) {
        MoveDown(Id);
      } else {
        if (x == 3) {
          MoveLeft(Id);
        } else {
          MoveRight(Id);
        }
      }
    }
  }
}
function CatAi(Id) {
  var Memory = [];
  var State = "Wander";
  appendItem(Memory, raycast(Id, 1, 0));
  appendItem(Memory, raycast(Id, -1, 0));
  appendItem(Memory, raycast(Id, 0, 1));
  appendItem(Memory, raycast(Id, 0, -1));
  appendItem(Memory, raycast(Id, 1, 1));
  appendItem(Memory, raycast(Id, 1, -1));
  appendItem(Memory, raycast(Id, -1, 1));
  appendItem(Memory, raycast(Id, -1, -1));
  for (var i = 0; i < Memory.length; i++) {
    if (Memory[i] == "c") {
      State = "Chase";
    }
  }
  if (State == "Chase") {
    for (var i = 0; i < Memory.length; i++) {
      if (Memory[i] == "s"||Memory[i] == "r") {
        if (i == 1) {
          MoveRight(Id);
        } else {
          if (i == 2) {
            MoveLeft(Id);
          } else {
            if (i == 3) {
              MoveDown(Id);
            } else {
              if (i == 4) {
                MoveUp(Id);
              } else {
                if (i == 5) {
                  MoveDown(Id);
                  MoveRight(Id);
                } else {
                  if (i == 6) {
                    MoveUp(Id);
                    MoveRight(Id);
                  } else {
                    if (i == 7) {
                      MoveUp(Id);
                      MoveLeft(Id);
                    } else {
                      MoveDown(Id);
                      MoveLeft(Id);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  }
  if (ThingTraits[Id].includes("Hungry")) {
  var Target = Track(Id, "s", 100);
  MoveToward(Id, Target);
  if (ThingX[Id] == ThingX[Target] && ThingY[Id] == ThingY[Target]) {
    Attack(Id, Target, 2, 2, 4);
    AddTrait(Target, "Immobile");
  }
  }else{
    // if not hungry, randomly make hungry so to be ocaionaly hungry
    if (1 == randomNumber(1, 1)) {
      AddTrait(Id, "Hungry");
    }
  }

  if (State == "Wander") {
    var x = randomNumber(1, 4);
    if (x == 1) {
      MoveUp(Id);
    } else {
      if (x == 2) {
        MoveDown(Id);
      } else {
        if (x == 3) {
          MoveLeft(Id);
        } else {
          MoveRight(Id);
        }
      }
    }
  }
}










function Immobile(Id) {
  console.log("message");
  SetX(Id, DecodeDouble1(PriorPos[Id]));
  SetY(Id, DecodeDouble2(PriorPos[Id]));
}

function Rodentivore(Id) {
        for (var i = 0; i < ThingChar.length; i++) {
        if ((ThingX[Id] - ThingX[i] < 2 && ThingX[Id] - ThingX[i] > -2)&&(ThingY[Id] - ThingY[i] < 2 && ThingY[Id] - ThingY[i] > -2)) {
          if (ThingChar[i] == "s" || ThingChar[i] == "r") {
            
          }
        }
  }
}

function SpawnRiver(Start) {
  var Broadness = 2;
  for (var i = 0; i < Broadness; i++) {
    var x = 0;
    var Xoffset = 0;
    for (var i2 = 0; i2 < Math.sqrt(MapSize); i2++) {
      var RiverSegment = SpawnThing("~");
      SetX(RiverSegment, Start+i+Xoffset);
      SetY(RiverSegment, x);
      x++;
      if (Math.round((i2+ Seed+45)/12) == (i2 + Seed+45)/12||Math.round((i2+ Seed+45)/7) == (i2 + Seed+45)/7) {
        Xoffset = Xoffset+1;
      }
    }
  }
  
}
function Attack(Id,Id2, range, Accuracy, Power) {
    console.log("message");
    if ((ThingX[Id] - ThingX[Id2] < range && ThingX[Id] - ThingX[Id2] > range*-1)&&(ThingY[Id] - ThingY[Id2] < range && ThingY[Id] - ThingY[Id2] > range*-1)) {
if (1 == randomNumber(1, Accuracy)) {
      var p = randomNumber(1, Power);
      var Parts = ["face", "leftforarm", "rightforarm", "ribs", "leftfoot", "rightfoot"];
      var part = Parts[randomNumber(0, Parts.length - 1)];
      var x = ThingTraits[Id];
      removeItem(ThingTraits, Id);
      if (p == 1) {
        insertItem(ThingTraits, Id, x + "Scratched" + part);
      }
      if (p == 2) {
        insertItem(ThingTraits, Id, x + "Bruised" + part);
      }
      if (p == 3) {
        insertItem(ThingTraits, Id, x + ("Cut" + part));
      }
      if (p == 4) {
        insertItem(ThingTraits, Id, x + ("Sliced" + part));
      }
      if (p == 5) {
        insertItem(ThingTraits, Id, x + (DeepBrusing + part));
      }
      if (p == 6) {
        insertItem(ThingTraits, Id, x + ("Fractured" + part));
      }
      if (p == 7) {
        insertItem(ThingTraits, Id, (x + "Brokenbone") + part);
      }
      if (p == 8) {
        insertItem(ThingTraits, Id, x + ("Mangled" + part));
      }
      if (p == 9) {
        insertItem(ThingTriats, Id, (x + "Deepcuts") + part);
      }
      if (p == 10) {
        insertItem(ThingTraits, Id, (x + "Missing") + part);
      }
    }
  }
}
function Track(Id,target,range) {
      //sets max distance can track
    var Preydist = range;
    for (var i = 0; i < ThingChar.length - 1; i++) {
      if (ThingChar[i] == target && Math.sqrt(Math.pow(ThingX[Id] - ThingX[i], 2) + Math.pow(ThingY[Id] - ThingY[i], 2)) < Preydist) {
        Preydist = Math.sqrt(Math.pow(ThingX[Id] - ThingX[i], 2) + Math.pow(ThingY[Id] - ThingY[i], 2));
        var PreyId = i;
      }
    }
      if (Preydist < range) {
        return PreyId;
      }
}
function MoveToward(Id, Id2) {
  if (Math.abs(ThingX[Id] - ThingX[Id2]) > Math.abs(ThingY[Id] - ThingY[Id2])) {
    if (ThingX[Id] - ThingX[Id2] > 0) {
      MoveLeft(Id);
    } else {
      MoveRight(Id);
    }
  } else {
    if (ThingY[Id] - ThingY[Id2] > 0) {
      MoveUp(Id);
    } else {
      MoveDown(Id);
    }
  }
}
function AddTrait(Id, trait) {
        var x = ThingTraits[Id];
        removeItem(ThingTraits, Id);
        insertItem(ThingTraits, Id, x + trait);
}
function RemoveTrait(Id, trait) {
   var x = ThingTraits[Id];
   if (x.includes(trait)) {
     x = x.substring(0, x.indexOf(trait))+x.substring(x.indexOf(trait) + trait.length, x.length);
   }
  removeItem(ThingTraits, Id);
  insertItem(ThingTraits, Id, x);
}
function NoClip(Id) {
  if (ThingX[Id] > Math.sqrt(MapSize)) {
  SetX(Id, Math.sqrt(MapSize));
}
if (ThingX[Id] < 0) {
  SetX(Id, 0);
}
if (ThingY[Id] > Math.sqrt(MapSize)) {
  SetY(Id, Math.sqrt(MapSize));
}
if (ThingY[Id] < 0) {
  SetY(Id, 0);
}
  for (var i = 0; i < ThingChar.length; i++) {
    if (ThingTraits[i].includes("Barrier")) {
      if (ThingX[i] == ThingX[Id] && ThingY[i] == ThingY[Id]) {
        //noclip Solution;
        if (DecodeDouble1(PriorPos[Id]) - ThingX[Id] > 0 && DecodeDouble2(PriorPos[Id]) - ThingY[Id] > 0) {
          MoveDown(Id);
          MoveLeft(Id);
        }
        if (DecodeDouble1(PriorPos[Id]) - ThingX[Id] > 0 && DecodeDouble2(PriorPos[Id]) - ThingY[Id] < 0) {
          MoveUp(Id);
          MoveLeft(Id);
        }
        if (DecodeDouble1(PriorPos[Id]) - ThingX[Id] < 0 && DecodeDouble2(PriorPos[Id]) - ThingY[Id] > 0) {
          MoveDown(Id);
          MoveRight(Id);
        }
        if (DecodeDouble1(PriorPos[Id]) - ThingX[Id] < 0 && DecodeDouble2(PriorPos[Id]) - ThingY[Id] < 0) {
          MoveUp(Id);
          MoveRight(Id);
        }
        if (DecodeDouble1(PriorPos[Id]) - ThingX[Id] == 0 && DecodeDouble2(PriorPos[Id]) - ThingY[Id] > 0) {
          MoveDown(Id);
        }
        if (DecodeDouble1(PriorPos[Id]) - ThingX[Id] > 0 && DecodeDouble2(PriorPos[Id]) - ThingY[Id] == 0) {
          MoveRight(Id);
        }
        if (DecodeDouble1(PriorPos[Id]) - ThingX[Id] == 0 && DecodeDouble2(PriorPos[Id]) - ThingY[Id] < 0) {
          MoveUp(Id);
        }
        if (DecodeDouble1(PriorPos[Id]) - ThingX[Id] < 0 && DecodeDouble2(PriorPos[Id]) - ThingY[Id] == 0) {
          MoveRight(Id);
        }
      }
    }
  }
}
function CheckIfInWater(Id) {
  if (!ThingTraits[Id].includes("Wet")) {
    for (var i = 0; i < ThingChar.length; i++) {
      if (ThingChar[i] == "~") {
        if (ThingX[Id] == ThingX[i] && ThingY[Id] == ThingY[i]) {
          AddTrait(Id, "Wet");
        }
      }
    }
  } else {
    if (randomNumber(1, 100) == 1) {
      RemoveTrait(Id, "Wet");
    }
  }
  
}
onEvent("button1", "click", function( ) {
  for (var i = 0; i < 1000; i++) {
    GameLoop();
  }
});
onEvent("screen1", "click", function( ) {
  GameLoop();
});
function EncodeDouble(v1, v2) {
  return (v1 + ("|" + v2));
}
function DecodeDouble1(Double) {
  return (Double.substring(0, Double.indexOf("|")));
}
function DecodeDouble2(Double) {
  return (Double.substring(Double.indexOf("|") + 1, Double.length));
}

