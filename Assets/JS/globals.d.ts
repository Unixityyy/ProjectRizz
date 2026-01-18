declare function log(message: any): void;
declare function tick(dt: number): void;

declare class Vector3 {
    x: number;
    y: number;
    z: number;
}

declare const LocalPlayer: PlayerBindings;

declare class PlayerBindings {
    VibrateIntensity: number;
    MaxJumpSpeed: number;
    JumpMultiplier: number;
    readonly CurrentVelocityAverage: Vector3;
    Position: Vector3;
    Material: number;
}